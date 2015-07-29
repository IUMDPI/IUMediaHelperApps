using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Packager.Attributes;
using Packager.Observers;
using Packager.Providers;
using RestSharp.Deserializers;

namespace Packager.Models.PodMetadataModels
{
    public class ConsolidatedPodMetadata
    {
        private PodMetadata Metadata { get; set; }
        private Details Details { get; set; }
        private Assignment Assignment { get; set; }
        private TechnicalMetadata TechnicalMetadata { get; set; }

        public ConsolidatedPodMetadata(PodMetadata metadata)
        {
            Metadata = metadata;
            Details = metadata.Data != null ? metadata.Data.Object.Details : null;
            Assignment = metadata.Data != null ? metadata.Data.Object.Assignment : null;
            TechnicalMetadata = metadata.Data != null ? metadata.Data.Object.TechnicalMetadata : null;
            DigitalProvenance = metadata.Data != null ? metadata.Data.Object.DigitalProvenance : null;
        }

        public DigitalProvenance DigitalProvenance { get; set; }

        public string Identifier { get { return Metadata.Data.Object.Details.Id; } }
        public bool Success { get { return Metadata.Success; } }
        public string Message { get { return Metadata.Message; } }
        public string CallNumber { get { return Details.CallNumber; } }
        public string Title { get { return Details.Title; } }
        public string Unit { get { return Assignment.Unit; } }
        public string Barcode { get { return Details.MdpiBarcode; } }
        public string Brand { get { return TechnicalMetadata.TapeStockBrand; } }
        public string CarrierType { get { return TechnicalMetadata.Format; } }
        public string DirectionsRecorded { get { return TechnicalMetadata.DirectionsRecorded; } }

        public string CleaningDate { get { return DigitalProvenance.CleaningDate; } }
        public string CleaningComment { get { return DigitalProvenance.CleaningComment; } }
        public string BakingDate { get { return DigitalProvenance.Baking; } }
        public bool Repaired { get { return DigitalProvenance.Repaired; } }

        public string DigitizingEntity { get { return DigitalProvenance.DigitizingEntity; } }

        public string PlaybackSpeeds
        {
            get { return GetBoolValuesAsList(TechnicalMetadata.PlaybackSpeed); }
        }

        public string TrackConfigurations
        {
            get { return GetBoolValuesAsList(TechnicalMetadata.TrackConfiguration); }
        }

        public string SoundFields { get { return GetBoolValuesAsList(TechnicalMetadata.SoundField); } }

        public string TapeThicknesses { get { return GetBoolValuesAsList(TechnicalMetadata.TapeThickness); } }


        private static string GetBoolValuesAsList(object instance)
        {
            if (instance == null)
            {
                return string.Empty;
            }

            var properties = instance.GetType().GetProperties().Where(p => p.PropertyType == typeof(bool));
            var results = (properties.Where(property => (bool)property.GetValue(instance))
                .Select(property => property.Name)).Distinct().ToList();
            return string.Join(", ", results);
        }
    }
}