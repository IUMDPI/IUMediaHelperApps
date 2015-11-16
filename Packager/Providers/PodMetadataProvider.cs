using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Validators;
using RestSharp;

namespace Packager.Providers
{
    public class PodMetadataProvider : IPodMetadataProvider
    {
        public PodMetadataProvider(IRestClient client, IObserverCollection observers, IValidatorCollection validators)
        {
            Client = client;
            Observers = observers;
            Validators = validators;
          
        }

        private IRestClient Client { get; }
        private IObserverCollection Observers { get; }
        private IValidatorCollection Validators { get; }
        
        public async Task<T> GetObjectMetadata<T>(string barcode) where T : AbstractPodMetadata, new()
        {
            var request = new RestRequest($"responses/objects/{barcode}/metadata/full/");
            
            var response = await Client.ExecuteGetTaskAsync<T>(request);

            VerifyResponse(response, "retrieve metadata from Pod");
            VerifyResponseMetadata(response.Data);

            return Normalize(response.Data);
        }

        public async Task<string> ResolveUnit(string unit)
        {
            var request = new RestRequest($"/responses/packager/units/{unit}");
            var response = await Client.ExecuteGetTaskAsync<BasePodResponse>(request);

            VerifyResponse(response, "resolve unit name using Pod");
            VerifyResponseMetadata(response.Data);

            return response.Data.Message;
        }

        public void Validate<T>(T podMetadata, List<ObjectFileModel> models) where T : AbstractPodMetadata
        {
            var results = ValidateMetadata(podMetadata);
            results.AddRange(ValidateMetadataProvenances(podMetadata.FileProvenances, models));

            if (results.Succeeded)
            {
                return;
            }

            throw new PodMetadataException(results);
        }

        public void Log<T>(T podMetadata) where T : AbstractPodMetadata
        {
            Observers.Log(podMetadata.GetStringPropertiesAndValues());
            Observers.Log(podMetadata.GetDatePropertiesAndValues());
            foreach (var provenance in podMetadata.FileProvenances)
            {
                var sectionKey = Observers.BeginSection("File Provenance: {0}", provenance.Filename.ToDefaultIfEmpty("[not set]"));
                Observers.Log(provenance.GetStringPropertiesAndValues("\t"));
                Observers.Log(provenance.GetDatePropertiesAndValues("\t"));

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

        private ValidationResults ValidateMetadata<T>(T podMetadata) where T : AbstractPodMetadata
        {
            return Validators.Validate(podMetadata);
        }

        private ValidationResults ValidateMetadataProvenances(List<AbstractDigitalFile> provenances, IEnumerable<ObjectFileModel> filesToProcess)
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


        // go through the result's string fields and remove leading and trailing whitespace
        private static T Normalize<T>(T metadata) where T : AbstractPodMetadata
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

        private static T NormalizeDateFields<T>(T metadata) where T : AbstractPodMetadata
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
                if (provenance.DateDigitized.HasValue == false)
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

        private static void VerifyResponse<T>(IRestResponse<T> response, string operation) where T : BasePodResponse
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new PodMetadataException(response.ErrorException, "Could not {0}", operation);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new PodMetadataException(response.ErrorException, "Could not {0}: {1} ({2})", operation, response.StatusCode, (int) response.StatusCode);
            }

            if (response.Data == null)
            {
                throw new PodMetadataException("Could not {0}: expected result object not present", operation);
            }

            if (response.Data.Success == false)
            {
                throw new PodMetadataException("Could not {0}: {1}", operation, response.Data.Message.ToDefaultIfEmpty("unknown issue returned from server"));
            }
        }


        private static void VerifyResponseMetadata(BasePodResponse metadata)
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
    }
}