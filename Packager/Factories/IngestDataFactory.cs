using System;
using System.Collections.Generic;
using System.Linq;
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
        public AbstractIngest Generate(DigitalAudioFile provenance)
        {
            return new AudioIngest
            {
                FileName = provenance.Filename,
                Comments = provenance.Comment,
                CreatedBy = provenance.CreatedBy,
                ExtractionWorkstation = new Device
                {
                    Model = provenance.ExtractionWorkstation.Model,
                    SerialNumber = provenance.ExtractionWorkstation.SerialNumber,
                    Manufacturer = provenance.ExtractionWorkstation.Manufacturer
                },
                SpeedUsed = provenance.SpeedUsed,
                Date = GetDateDigitized(provenance.DateDigitized, "yyyy-MM-dd"),
                Stylus = provenance.StylusSize,
                Turnover = provenance.Turnover,
                ReferenceFluxivity = provenance.ReferenceFluxivity,
                Gain = provenance.Gain,
                AnalogOutputVoltage = provenance.AnalogOutputVoltage,
                Peak = provenance.Peak,
                Rolloff = provenance.Rolloff,
                Players = provenance.PlayerDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                AdDevices = provenance.AdDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                PreAmpDevices = provenance.PreampDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray()
            };
        }

        public AbstractIngest Generate(DigitalVideoFile provenance)
        {
            return new VideoIngest
            {
                FileName = provenance.Filename,
                Comments = provenance.Comment,
                CreatedBy = provenance.CreatedBy,
                ExtractionWorkstation = new Device
                {
                    Model = provenance.ExtractionWorkstation.Model,
                    SerialNumber = provenance.ExtractionWorkstation.SerialNumber,
                    Manufacturer = provenance.ExtractionWorkstation.Manufacturer
                },
                Date = GetDateDigitized(provenance.DateDigitized, "yyyy-MM-dd"),
                Players = provenance.PlayerDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                AdDevices = provenance.AdDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                TbcDevices = provenance.TBCDevices
                    .Select(
                        d => new Device {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                Encoder = new Device
                {
                    Model = provenance.Encoder.Model,
                    SerialNumber = provenance.Encoder.SerialNumber,
                    Manufacturer = provenance.Encoder.Manufacturer
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
    }
}