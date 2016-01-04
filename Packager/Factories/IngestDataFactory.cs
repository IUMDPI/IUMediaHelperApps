using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class IngestDataFactory : IIngestDataFactory
    {
        public IngestData Generate(AudioPodMetadata podMetadata, AbstractFileModel masterFileModel)
        {
            var digitalFileProvenance = podMetadata.FileProvenances.GetFileProvenance(masterFileModel) as DigitalAudioFile;
            if (digitalFileProvenance == null)
            {
                throw new OutputXmlException("No digital file provenance found for {0}", masterFileModel.ToFileName());
            }

            return new IngestData
            {
                XsiType = $"{podMetadata.Format}Ingest".RemoveSpaces(),
                
                Comments = digitalFileProvenance.Comment,
                CreatedBy = digitalFileProvenance.CreatedBy,
                ExtractionWorkstation = new IngestDevice
                {
                    Model = digitalFileProvenance.ExtractionWorkstation.Model,
                    SerialNumber = digitalFileProvenance.ExtractionWorkstation.SerialNumber,
                    Manufacturer = digitalFileProvenance.ExtractionWorkstation.Manufacturer
                },
                SpeedUsed = digitalFileProvenance.SpeedUsed,
                Date = digitalFileProvenance.DateDigitized.ToString(),
                Stylus = digitalFileProvenance.StylusSize,
                Turnover = digitalFileProvenance.Turnover,
                ReferenceFluxivity = digitalFileProvenance.ReferenceFluxivity,
                Gain = digitalFileProvenance.Gain,
                AnalogOutputVoltage = digitalFileProvenance.AnalogOutputVoltage,
                Peak = digitalFileProvenance.Peak,
                Rolloff = digitalFileProvenance.Rolloff,
                Players = digitalFileProvenance.PlayerDevices
                    .Select(d=>new IngestDevice {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                AdDevices = digitalFileProvenance.AdDevices
                    .Select(d => new IngestDevice { Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer })
                    .ToArray()
            };

            
        }
    }
}