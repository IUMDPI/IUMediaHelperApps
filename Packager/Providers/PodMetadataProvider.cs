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
        public PodMetadataProvider(IProgramSettings programSettings, IObserverCollection observers, IValidatorCollection validators)
        {
            ProgramSettings = programSettings;
            Observers = observers;
            Validators = validators;
        }

        private IProgramSettings ProgramSettings { get; }
        private IObserverCollection Observers { get; }
        private IValidatorCollection Validators { get; }

        public async Task<ConsolidatedPodMetadata> GetObjectMetadata(string barcode)
        {
            var client = new RestClient(ProgramSettings.WebServiceUrl)
            {
                Authenticator =
                    new HttpBasicAuthenticator(ProgramSettings.PodAuth.UserName, ProgramSettings.PodAuth.Password)
            };


            var request = new RestRequest($"responses/objects/{barcode}/metadata/full/");

            var response = await client.ExecuteGetTaskAsync<PodMetadata>(request);

            VerifyResponse(response, "retrieve metadata from Pod");
            VerifyResponseMetadata(response.Data);
            VerifyResponseObjectsPresent(response.Data);

            return ConsolidateMetadata(response.Data);
        }

        public async Task<string> ResolveUnit(string unit)
        {
            var client = new RestClient(ProgramSettings.WebServiceUrl)
            {
                Authenticator =
                    new HttpBasicAuthenticator(ProgramSettings.PodAuth.UserName, ProgramSettings.PodAuth.Password)
            };

            var request = new RestRequest($"/responses/packager/units/{unit}");

            var response = await client.ExecuteGetTaskAsync<PodMetadata>(request);

            VerifyResponse(response, "resolve unit name using Pod");
            VerifyResponseMetadata(response.Data);
            
            return response.Data.Message;
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

        private static ConsolidatedPodMetadata ConsolidateMetadata(PodMetadata metadata)
        {
            var result = new ConsolidatedPodMetadata
            {
                Identifier = metadata.Data.Object.Details.Id,
                Format = metadata.Data.Object.Details.Format,
                CallNumber = metadata.Data.Object.Details.CallNumber,
                Title = metadata.Data.Object.Details.Title,
                Unit = metadata.Data.Object.Assignment.Unit,
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

            result = NormalizeDateFields(result);
            return NormalizeResultFields(result);
        }

        // go through the result's string fields and remove leading and trailing whitespace
        private static ConsolidatedPodMetadata NormalizeResultFields(ConsolidatedPodMetadata metadata)
        {
            metadata = NormalizeFields(metadata);

            for (var i = 0; i < metadata.FileProvenances.Count; i++)
            {
                metadata.FileProvenances[i] = NormalizeFields(metadata.FileProvenances[i]);

                for (var j = 0; j < metadata.FileProvenances[i].SignalChain.Count; j++)
                {
                    metadata.FileProvenances[i].SignalChain[j] = NormalizeFields(metadata.FileProvenances[i].SignalChain[j]);
                }
            }

            return metadata;
        }

        private static ConsolidatedPodMetadata NormalizeDateFields(ConsolidatedPodMetadata metadata)
        {
            if (metadata.BakingDate.HasValue)
            {
                metadata.BakingDate = metadata.BakingDate.Value.ToUniversalTime();
            }

            if (metadata.CleaningDate.HasValue)
            {
                metadata.CleaningDate = metadata.CleaningDate.Value.ToUniversalTime();
            }

            foreach (var provenance in metadata.FileProvenances)
            {
                if (provenance.DateDigitized.HasValue ==false)
                {
                    continue;
                }

                provenance.DateDigitized = provenance.DateDigitized.Value.ToUniversalTime();
            }

            return metadata;
        }

        private static T NormalizeFields<T>(T value)
        {
            foreach (var property in value.GetType().GetProperties().Where(p => p.PropertyType == typeof (string) && p.CanWrite))
            {
                property.SetValue(value, ((string) property.GetValue(value)).TrimWhiteSpace());
            }

            return value;
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

        private static void VerifyResponse(IRestResponse response, string operation)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new PodMetadataException(response.ErrorException, "Could not {0}", operation);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new PodMetadataException(response.ErrorException, "Could not {0}: {1} ({2})", operation, response.StatusCode, (int) response.StatusCode);
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
        }

        private static void VerifyResponseObjectsPresent(PodMetadata metadata)
        {
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