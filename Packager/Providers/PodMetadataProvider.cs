using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
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

        public async Task<T> GetObjectMetadata<T>(string barcode, CancellationToken cancellationToken) where T : AbstractPodMetadata, new()
        {
            var request = new RestRequest($"responses/objects/{barcode}/metadata/digital_provenance");

            var response = await Client.ExecuteGetTaskAsync<T>(request, cancellationToken);
            VerifyResponse(response);

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

        private static ValidationResults ValidateMetadataProvenances(List<AbstractDigitalFile> provenances,
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
                .Select(m => m.Filename).ToList();

            var presentFiles = filesToProcess
                .Select(m => m.Filename).ToList();

            results.AddRange(
                expectedMasters.Except(presentFiles)
                    .Select(f => new ValidationResult("No original master present for {0}", f)));
            return results;
        }

        private static ValidationResults ValidateProvenancesPresent(List<AbstractDigitalFile> provenances,
            IEnumerable<AbstractFile> filesToProcess)
        {
            var results = new ValidationResults();

            var unexpectedMasters = filesToProcess
                .Where(m => m.IsPreservationVersion() || m.IsPreservationIntermediateVersion())
                .Where(m => m.ShouldNormalize)
                .Select(m => new { key = m.Filename, value = provenances.GetFileProvenance(m) })
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
                    value.GetType().GetProperties().Where(p => p.PropertyType == typeof(string) && p.CanWrite))
            {
                property.SetValue(value, ((string)property.GetValue(value)).TrimWhiteSpace());
            }

            return value;
        }

        private static void VerifyResponse<T>(IRestResponse<T> response) where T : AbstractPodMetadata
        {
            CheckForInternalIssue(response);
            CheckForServerIssue(response);
            CheckForMetadataIssue(response);
        }

        private static void CheckForInternalIssue<T>(IRestResponse<T> response)
            where T : AbstractPodMetadata
        {
            if (response.StatusCode != 0 || response.ResponseStatus == ResponseStatus.Completed)
            {
                return;
            }

            var reportedException = response.ErrorException ?? new Exception("an unknown issue occurred");

            throw new PodMetadataException(response.ErrorException,
                $"Could not retrieve metadata from POD: an internal issue occurred\n--- {reportedException}");
        }

        private static void CheckForServerIssue<T>(IRestResponse<T> response)
            where T : AbstractPodMetadata
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            var message = $"Could not retrieve metadata from POD: {response.StatusCode} ({(int)response.StatusCode})";
            if (ResponseMessageSet(response))
            {
                message = $"{message}\n--- {response.Data.Message}";
            }

            throw new PodMetadataException(response.ErrorException, message);
        }

        private static bool ResponseMessageSet<T>(IRestResponse<T> response)
            where T : AbstractPodMetadata
        {
            return response.Data != null && response.Data.Message.IsSet();
        }

        private static void CheckForMetadataIssue<T>(IRestResponse<T> response)
            where T : AbstractPodMetadata
        {
            if (response.Data != null && response.Data.Success)
            {
                return;
            }

            var message = ResponseMessageSet(response)
                // ReSharper disable once PossibleNullReferenceException (ResponseMessageSet checks for null)
                ? $"Could not retrieve metadata from POD: {response.Data.Message}"
                : "Could not retrieve metadata from POD: metadata element could not be resolved";

            throw new PodMetadataException(message);

        }

        public T AdjustMediaFormat<T>(T podMetadata, List<AbstractFile> models) where T : AbstractPodMetadata
        {
            if (models.Any(m => IsIreneMaster(m)))
            {
                podMetadata.Format = GetIreneFormat(podMetadata.Format);
            }

            return podMetadata;
        }

        private static bool IsIreneMaster(AbstractFile model)
        {
            return model.FileUsage == FileUsages.PreservationMaster &&
                   model.Extension.ToDefaultIfEmpty().Equals(".zip", StringComparison.InvariantCultureIgnoreCase);
        }

        private static IMediaFormat GetIreneFormat(IMediaFormat format)
        {
            if (format != MediaFormats.LacquerDisc)
            {
                throw new PodMetadataException($"No Irene-compatible format found for {format}");
            }

            return MediaFormats.LacquerDiscIrene;
        }

        public T AdjustDigitalProvenanceData<T>(T podMetadata, List<AbstractFile> models) where T : AbstractPodMetadata
        {
            var ireneProvenances = models
                .Where(m => IsIreneMaster(m))
                .Select(m => CloneProvenance(m, podMetadata.FileProvenances))
                .Where(p => podMetadata.FileProvenances.Any(pm => pm.Filename == p.Filename) == false)
                .ToList();

            podMetadata.FileProvenances.AddRange(ireneProvenances);

            return podMetadata;
        }

        private AbstractDigitalFile CloneProvenance(AbstractFile master, List<AbstractDigitalFile> provenances)
        {
            var matchingFilename = master.ConvertTo<AudioPreservationIntermediateFile>().Filename;
            var matchingProvenance = provenances.SingleOrDefault(p => p.Filename.Equals(matchingFilename)) as DigitalAudioFile;

            if (matchingProvenance == null)
            {
                throw new PodMetadataException(
                    $"Could not backfill .zip (Irene) master provenance; No provenance present for {matchingFilename}");
            }

            return new DigitalAudioFile
            {
                Filename = master.Filename,
                SpeedUsed = matchingProvenance?.SpeedUsed,
                ReferenceFluxivity = matchingProvenance?.ReferenceFluxivity,
                AnalogOutputVoltage = matchingProvenance?.AnalogOutputVoltage,
                Peak = matchingProvenance?.Peak,
                StylusSize = matchingProvenance?.StylusSize,
                Turnover = matchingProvenance?.Turnover,
                Gain = matchingProvenance?.Gain,
                Rolloff = matchingProvenance?.Rolloff,
                DateDigitized = matchingProvenance?.DateDigitized,
                CreatedBy = matchingProvenance?.CreatedBy,
                Comment = matchingProvenance?.Comment,
                SignalChain = matchingProvenance?.SignalChain,
            };

        }
    }
}