using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Validators;
using RestSharp;
using RestSharp.Authenticators;

namespace Packager.Providers
{
    internal class PodMetadataProvider : IPodMetadataProvider
    {
        public PodMetadataProvider(IProgramSettings programSettings, ILookupsProvider lookupsProvider, IObserverCollection observers, IValidatorCollection validators)
        {
            ProgramSettings = programSettings;
            LookupsProvider = lookupsProvider;
            Observers = observers;
            Validators = validators;
        }

        private IProgramSettings ProgramSettings { get; }
        private ILookupsProvider LookupsProvider { get; }
        private IObserverCollection Observers { get; }
        private IValidatorCollection Validators { get; }

        public async Task<ConsolidatedPodMetadata> Get(string barcode)
        {
            var client = new RestClient(ProgramSettings.WebServiceUrl)
            {
                Authenticator =
                    new HttpBasicAuthenticator(ProgramSettings.PodAuth.UserName, ProgramSettings.PodAuth.Password)
            };


            var request = new RestRequest($"responses/objects/{barcode}/metadata/full/")
            {
                DateFormat = "yyyy-MM-dd HH:mm:ss zzz"
            };
            var response = await client.ExecuteGetTaskAsync<PodMetadata>(request);

            VerifyResponse(response);
            VerifyResponseMetadata(response.Data);

            return ConsolidateMetadata(response.Data);
        }

        public void Validate(ConsolidatedPodMetadata podMetadata, List<ObjectFileModel> models)
        {
            var results = ValidateMetadata(podMetadata);
            results.AddRange(ValidateMetadataProvenances(podMetadata.FileProvenances, models));

            if (results.Succeeded)
            {
                return;
            }

            throw new PodMetadataException(results);
        }

        public void Log(ConsolidatedPodMetadata podMetadata)
        {
            Observers.Log(podMetadata.GetStringPropertiesAndValues());

            foreach (var provenance in podMetadata.FileProvenances)
            {
                var sectionKey = Observers.BeginSection("File Provenance: {0}", provenance.Filename.ToDefaultIfEmpty("[not set]"));
                Observers.Log(provenance.GetStringPropertiesAndValues("\t"));

                foreach (var device in provenance.PlayerDevices)
                {
                    Observers.Log("\tPlayer: {0} {1} ({2})", device.Manufacturer, device.Model, device.SerialNumber);
                }

                foreach (var device in provenance.AdDevices)
                {
                    Observers.Log("\tAD Device: {0} {1} ({2})", device.Manufacturer, device.Model, device.SerialNumber);
                }

                Observers.EndSection(sectionKey);
            }
        }

        private ValidationResults ValidateMetadata(ConsolidatedPodMetadata podMetadata)
        {
            return Validators.Validate(podMetadata);
        }

        private ValidationResults ValidateMetadataProvenances(List<DigitalFileProvenance> provenances, IEnumerable<ObjectFileModel> filesToProcess)
        {
            var results = new ValidationResults();
            if (provenances == null)
            {
                results.Add("Value not set for digital file provenances");
                return results;
            }

            // make sure that all preservation masters 
            // have digital file provenance data in metadata
            foreach (var model in filesToProcess.Where(m => m.IsPreservationVersion() || m.IsPreservationIntermediateVersion()))
            {
                var provenance = provenances.GetFileProvenance(model);
                if (provenance == null)
                {
                    results.Add("No digital file provenance found for {0}", model.ToFileName());
                    continue;
                }

                results.AddRange(Validators.Validate(provenance));
            }

            return results;
        }

        private ConsolidatedPodMetadata ConsolidateMetadata(PodMetadata metadata)
        {
            var result = new ConsolidatedPodMetadata
            {
                Identifier = metadata.Data.Object.Details.Id,
                Format = metadata.Data.Object.Details.Format,
                CallNumber = metadata.Data.Object.Details.CallNumber,
                Title = metadata.Data.Object.Details.Title,
                Unit = LookupsProvider.LookupValue(LookupTables.Units, metadata.Data.Object.Assignment.Unit),
                Barcode = metadata.Data.Object.Details.MdpiBarcode,
                Brand = metadata.Data.Object.TechnicalMetadata.TapeStockBrand,
                DirectionsRecorded = metadata.Data.Object.TechnicalMetadata.DirectionsRecorded,
                CleaningDate = metadata.Data.Object.DigitalProvenance.CleaningDate,
                CleaningComment = metadata.Data.Object.DigitalProvenance.CleaningComment,
                BakingDate = metadata.Data.Object.DigitalProvenance.Baking,
                Repaired = ToYesNo(metadata.Data.Object.DigitalProvenance.Repaired),
                DigitizingEntity = metadata.Data.Object.DigitalProvenance.DigitizingEntity,
                PlaybackSpeed = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.PlaybackSpeed),
                TrackConfiguration = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.TrackConfiguration),
                SoundField = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.SoundField),
                TapeThickness = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.TapeThickness),
                FileProvenances = metadata.Data.Object.DigitalProvenance.DigitalFiles,
                Damage = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.Damage, "None"),
                PreservationProblems = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.PreservationProblems)
            };

            return result;
        }

        private static string GetBoolValuesAsList(object instance, string defaultValue = "")
        {
            if (instance == null)
            {
                return defaultValue;
            }

            var properties = instance.GetType().GetProperties().Where(p => p.PropertyType == typeof (bool));
            var results = (properties.Where(property => (bool) property.GetValue(instance))
                .Select(GetNameOrDescription)).Distinct().ToList();

            return results.Any()
                ? string.Join(", ", results)
                : defaultValue;
        }

        private static string GetNameOrDescription(MemberInfo propertyInfo)
        {
            var description = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
            return description == null ? propertyInfo.Name : description.Description;
        }

        private static string ToYesNo(bool? value)
        {
            if (value.HasValue == false)
            {
                return "No";
            }
            return value.Value ? "Yes" : "No";
        }

        private static void VerifyResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new PodMetadataException(response.ErrorException, "Could not retrieve metadata from Pod");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new PodMetadataException(response.ErrorException, "Could not retrieve metadata from Pod: {0} ({1})", response.StatusCode, (int) response.StatusCode);
            }
        }

        private static void VerifyResponseMetadata(PodMetadata metadata)
        {
            if (metadata == null)
            {
                throw new PodMetadataException("Could not retrieve metadata from Pod");
            }

            if (metadata.Success == false)
            {
                throw new PodMetadataException(
                    "Could not retrieve metadata: {0}",
                    metadata.Message.ToDefaultIfEmpty("[no error message present]"));
            }

            if (metadata.Data == null ||
                metadata.Data.Object == null ||
                metadata.Data.Object.Assignment == null ||
                metadata.Data.Object.Basics == null ||
                metadata.Data.Object.Details == null ||
                metadata.Data.Object.DigitalProvenance == null ||
                metadata.Data.Object.TechnicalMetadata == null)
            {
                throw new PodMetadataException("Could not retrieve metadata from Pod: required sub-section not present");
            }
        }
    }
}