using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
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
            var request = new RestRequest($"responses/objects/{barcode}/metadata/digital_provenance");

            var response = await Client.ExecuteGetTaskAsync<T>(request);

            VerifyResponse(response, "retrieve metadata from Pod");
            VerifyResponseMetadata(response.Data);

            return Normalize(response.Data);
        }

        public void Validate<T>(T podMetadata, List<AbstractFile> models) where T : AbstractPodMetadata
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
                var sectionKey = Observers.BeginSection("File Provenance: {0}",
                    provenance.Filename.ToDefaultIfEmpty("[not set]"));
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

        private ValidationResults ValidateMetadataProvenances(List<AbstractDigitalFile> provenances,
            List<AbstractFile> filesToProcess)
        {
            var results = new ValidationResults();
            if (provenances == null)
            {
                results.Add("Value not set for digital file provenances");
                return results;
            }

            results.AddRange(ValidateMastersPresent(provenances, filesToProcess));
            results.AddRange(ValidateProvenancesPresent(provenances, filesToProcess));

            return results;
        }

        private static ValidationResults ValidateMastersPresent(IEnumerable<AbstractDigitalFile> provenances,
            IEnumerable<AbstractFile> filesToProcess)
        {
            var results = new ValidationResults();

            var expectedMasters = provenances
                .Select(p => FileModelFactory.GetModel(p.Filename))
                .Select(m => m.ToFileName()).ToList();

            var presentMasters = filesToProcess
                .Where(m => m.IsPreservationVersion() || m.IsPreservationIntermediateVersion())
                .Select(m => m.ToFileName()).ToList();

            results.AddRange(
                expectedMasters.Except(presentMasters)
                    .Select(f => new ValidationResult("No original master present for {0}", f)));
            return results;
        }

        private static ValidationResults ValidateProvenancesPresent(List<AbstractDigitalFile> provenances,
            IEnumerable<AbstractFile> filesToProcess)
        {
            var results = new ValidationResults();

            var unexpectedMasters = filesToProcess.Where(
                m => m.IsPreservationVersion() || m.IsPreservationIntermediateVersion())
                .Select(m => new {key = m.ToFileName(), value = provenances.GetFileProvenance(m)})
                .Where(p => p.value == null)
                .Select(p => p.key);

            results.AddRange(
                unexpectedMasters.Select(f => new ValidationResult("No digital file provenance found for {0}", f)));
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
                    metadata.FileProvenances[i].SignalChain[j] =
                        NormalizeFields(metadata.FileProvenances[i].SignalChain[j]);
                }
            }

            return metadata;
        }
        
        private static T NormalizeFields<T>(T value)
        {
            foreach (
                var property in
                    value.GetType().GetProperties().Where(p => p.PropertyType == typeof (string) && p.CanWrite))
            {
                property.SetValue(value, ((string) property.GetValue(value)).TrimWhiteSpace());
            }

            return value;
        }

        private static void VerifyResponse<T>(IRestResponse<T> response, string operation) where T : AbstractPodMetadata
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new PodMetadataException(response.ErrorException, "Could not {0}", operation);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new PodMetadataException(response.ErrorException, "Could not {0}: {1} ({2})", operation,
                    response.StatusCode, (int) response.StatusCode);
            }

            if (response.Data == null)
            {
                throw new PodMetadataException("Could not {0}: expected result object not present", operation);
            }

            if (response.Data.Success == false)
            {
                throw new PodMetadataException("Could not {0}: {1}", operation,
                    response.Data.Message.ToDefaultIfEmpty("unknown issue returned from server"));
            }
        }


        private static void VerifyResponseMetadata(AbstractPodMetadata metadata)
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