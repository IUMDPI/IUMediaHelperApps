using System;
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
        public int DirectionsRecorded { get; set; }

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

        public static CarrierData FromPodMetadata(PodMetadata podMetadata)
        {
            return new CarrierData
            {
                Barcode = GetBarcode(podMetadata), 
                Brand = GetBrand(podMetadata),
                CarrierType = GetCarrierType(podMetadata),
                DirectionsRecorded = GetDirectionsRecorded(podMetadata), 
                Identifier = GetIdentifier(podMetadata),
                Configuration = ConfigurationData.FromPodMetadata(podMetadata),
                Thickness = podMetadata.Data.Object.TechnicalMetadata.TapeThickness.GetValue()
            };
        }

        private static string GetCarrierType(PodMetadata podMetadata)
        {
            return podMetadata.Data.Object.Basics.Format;
        }

        private static string GetBarcode(PodMetadata podMetadata)
        {
            return podMetadata.Data.Object.Details.MdpiBarcode;
        }

        private static int GetDirectionsRecorded(PodMetadata podMetadata)
        {
            return podMetadata.Data.Object.TechnicalMetadata.DirectionsRecorded;
        }

        private static string GetBrand(PodMetadata podMetadata)
        {
            return podMetadata.Data.Object.TechnicalMetadata.TapeStockBrand;
        }

        private static string GetIdentifier(PodMetadata podMetadata)
        {
            return "Unknown";
        }
    }
}