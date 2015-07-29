using System;
using System.Linq;
using System.Xml.Serialization;
using Packager.Models.PodMetadataModels;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class CarrierData
    {
        [XmlAttribute("type")]
        public string CarrierType { get; set; }

        // todo: figure out where to get this from
        [XmlElement(Order = 1)]
        public string Identifier { get; set; }

        [XmlElement(Order = 2)]
        public string Barcode { get; set; }

        [XmlElement(Order = 3)]
        public ConfigurationData Configuration { get; set; }

        [XmlElement(Order = 4)]
        public string Brand { get; set; }

        [XmlElement(Order = 5)]
        public string Thickness { get; set; }

        [XmlElement(Order = 6)]
        public string DirectionsRecorded { get; set; }

        [XmlElement(Order = 7)]
        public PhysicalConditionData PhysicalCondition { get; set; }

        [XmlElement(Order = 8)]
        public string Repaired { get; set; }

        [XmlElement(Order = 9)]
        public PreviewData Preview { get; set; }

        [XmlElement(Order = 10)]
        public CleaningData Cleaning { get; set; }

        [XmlElement(Order = 11)]
        public BakingData Baking { get; set; }

        [XmlElement(Order = 12)]
        public PartsData Parts { get; set; }

        public static CarrierData FromPodMetadata(ConsolidatedPodMetadata podMetadata)
        {
            return new CarrierData
            {
                Barcode = podMetadata.Barcode,
                Brand = podMetadata.Brand,
                CarrierType = podMetadata.CarrierType,
                DirectionsRecorded = podMetadata.DirectionsRecorded,
                Identifier = podMetadata.Identifier,
                Configuration = ConfigurationData.FromPodMetadata(podMetadata),
                Thickness = podMetadata.TapeThicknesses,
                Baking = new BakingData { Date = podMetadata.BakingDate},
                Cleaning = new CleaningData { Date = podMetadata.CleaningDate},
                Repaired = podMetadata.Repaired? "Yes":"No",
                PhysicalCondition = new PhysicalConditionData
                {
                    
                }
            };
        }
    }
}