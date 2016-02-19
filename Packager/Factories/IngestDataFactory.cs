using System;
using System.Linq;
using System.Xml;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels.Ingest;
using Packager.Models.PodMetadataModels;
using Device = Packager.Models.OutputModels.Ingest.Device;

namespace Packager.Factories
{
    public class IngestDataFactory : IIngestDataFactory
    {
        public VideoIngest Generate(VideoPodMetadata podMetadata, AbstractFile masterFileModel)
        {
            var digitalFileProvenance =
                podMetadata.FileProvenances.GetFileProvenance(masterFileModel) as DigitalVideoFile;
            if (digitalFileProvenance == null)
            {
                throw new OutputXmlException("No digital file provenance found for {0}", masterFileModel.Filename);
            }

            return new VideoIngest
            {
                Comments = digitalFileProvenance.Comment,
                CreatedBy = digitalFileProvenance.CreatedBy,
                ExtractionWorkstation = new Device
                {
                    Model = digitalFileProvenance.ExtractionWorkstation.Model,
                    SerialNumber = digitalFileProvenance.ExtractionWorkstation.SerialNumber,
                    Manufacturer = digitalFileProvenance.ExtractionWorkstation.Manufacturer
                },
                DigitStatus = "OK", // todo: confirm is valid
                Date = GetDateDigitized(digitalFileProvenance.DateDigitized, "yyyy-MM-dd"),
                Players = digitalFileProvenance.PlayerDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                AdDevices = digitalFileProvenance.AdDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                TbcDevices = digitalFileProvenance.TBCDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                Encoder = new Device
                {
                    Model = digitalFileProvenance.Encoder.Model,
                    SerialNumber = digitalFileProvenance.Encoder.SerialNumber,
                    Manufacturer = digitalFileProvenance.Encoder.Manufacturer
                }
            };
        }

        private static string GetDateDigitized(DateTime? date, string format)
        {
            if (!date.HasValue)
            {
                throw new OutputXmlException("Date digitized is not set");
            }

            return date.Value.ToUniversalTime().ToString(format);
        }

        public AudioIngest Generate(AudioPodMetadata podMetadata, AbstractFile masterFileModel)
        {
            var digitalFileProvenance =
                podMetadata.FileProvenances.GetFileProvenance(masterFileModel) as DigitalAudioFile;
            if (digitalFileProvenance == null)
            {
                throw new OutputXmlException("No digital file provenance found for {0}", masterFileModel.Filename);
            }

            return new AudioIngest
            {
                Comments = digitalFileProvenance.Comment,
                CreatedBy = digitalFileProvenance.CreatedBy,
                ExtractionWorkstation = new Device
                {
                    Model = digitalFileProvenance.ExtractionWorkstation.Model,
                    SerialNumber = digitalFileProvenance.ExtractionWorkstation.SerialNumber,
                    Manufacturer = digitalFileProvenance.ExtractionWorkstation.Manufacturer
                },
                SpeedUsed = digitalFileProvenance.SpeedUsed,
                Date = GetDateDigitized(digitalFileProvenance.DateDigitized, "yyyy-MM-dd"),
                Stylus = digitalFileProvenance.StylusSize,
                Turnover = digitalFileProvenance.Turnover,
                ReferenceFluxivity = digitalFileProvenance.ReferenceFluxivity,
                Gain = digitalFileProvenance.Gain,
                AnalogOutputVoltage = digitalFileProvenance.AnalogOutputVoltage,
                Peak = digitalFileProvenance.Peak,
                Rolloff = digitalFileProvenance.Rolloff,
                Players = digitalFileProvenance.PlayerDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                AdDevices = digitalFileProvenance.AdDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                PreAmpDevices =  digitalFileProvenance.PreampDevices
                    .Select(
                        d=> new Device { Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer })
                    .ToArray()
            };
        }
    }
}