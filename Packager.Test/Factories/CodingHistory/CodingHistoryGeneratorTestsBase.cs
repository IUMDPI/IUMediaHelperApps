using System.Collections.Generic;
using NUnit.Framework;
using Packager.Extensions;
using Packager.Factories.CodingHistory;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Factories.CodingHistory
{
    public abstract class CodingHistoryGeneratorTestsBase
    {
        private const string Barcode = "4890764553278906";

        protected AudioPodMetadata Metadata { get; set; }
        protected AbstractCodingHistoryGenerator Generator { get;  set; }
        protected DigitalAudioFile Provenance { get; private set; }
        protected static AbstractFile BaseModel => new UnknownFile($"MDPI_{Barcode}_01.wav");
        protected static AudioPreservationFile PresModel => BaseModel.ConvertTo<AudioPreservationFile>();
        protected static AudioPreservationIntermediateFile PresIntModel => BaseModel.ConvertTo<AudioPreservationIntermediateFile>();
        protected static ProductionFile ProdModel => BaseModel.ConvertTo<ProductionFile>();

        [SetUp]
        public virtual void BeforeEach()
        {
            Provenance = new DigitalAudioFile
            {
                SignalChain = GetSignalChain()
            };
        }


        private static List<Device> GetSignalChain()
        {
            var result = new List<Device>
            {
                new Device
                {
                    DeviceType = "extraction workstation",
                    Model = "ew model",
                    Manufacturer = "ew manufacturer",
                    SerialNumber = "ew serial number"
                },
                new Device
                {
                    DeviceType = "player",
                    Model = "Player model",
                    Manufacturer = "Player manufacturer",
                    SerialNumber = "Player serial number"
                },
                new Device
                {
                    DeviceType = "ad",
                    Model = "Ad model",
                    Manufacturer = "Ad manufacturer",
                    SerialNumber = "Ad serial number"
                }
            };

            return result;
        }
    }
}