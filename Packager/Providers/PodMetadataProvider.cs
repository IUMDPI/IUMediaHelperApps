using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.PodMetadataModels.ConsolidatedModels;
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

        public async Task<T> GetObjectMetadata<T>(string barcode) where T : AbstractConsolidatedPodMetadata, new()
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

            return ConsolidateMetadata<T>(response.Data);
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

        public void Validate<T>(T podMetadata, List<ObjectFileModel> models) where T : AbstractConsolidatedPodMetadata
        {
            var results = ValidateMetadata(podMetadata);
            results.AddRange(ValidateMetadataProvenances(podMetadata.FileProvenances, models));

            if (results.Succeeded)
            {
                return;
            }

            throw new PodMetadataException(results);
        }

        public void Log<T>(T podMetadata) where T : AbstractConsolidatedPodMetadata
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

        private ValidationResults ValidateMetadata<T>(T podMetadata) where T : AbstractConsolidatedPodMetadata
        {
            return Validators.Validate(podMetadata);
        }

        private ValidationResults ValidateMetadataProvenances(List<AbstractConsolidatedDigitalFile> provenances, IEnumerable<ObjectFileModel> filesToProcess)
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

        private static T ConsolidateMetadata<T>(PodMetadata metadata) where T : AbstractConsolidatedPodMetadata, new()
        {
            var result = new T();
            result.ImportFromFullMetadata(metadata);
            return NormalizeResultFields(result);
        }

        // go through the result's string fields and remove leading and trailing whitespace
        private static T NormalizeResultFields<T>(T metadata) where T : AbstractConsolidatedPodMetadata
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

        private static T NormalizeFields<T>(T value)
        {
            foreach (var property in value.GetType().GetProperties().Where(p => p.PropertyType == typeof (string) && p.CanWrite))
            {
                property.SetValue(value, ((string) property.GetValue(value)).TrimWhiteSpace());
            }

            return value;
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