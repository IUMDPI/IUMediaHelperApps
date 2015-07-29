using System.Collections.Generic;

namespace Packager.Models.PodMetadataModels
{
    namespace Packager.Models.PodMetadataModels
    {
        public class PodMetadataOld
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public PodData Data { get; set; }
        }

        public class PodData
        {
            public PodDataObject Object { get; set; }
        }

        public class PodDataObject
        {
            public PodDataObjectBasics Basics { get; set; }
            public PodDataObjectAssignment Assignment { get; set; }
            public PodDataObjectDetails Details { get; set; }
            public PodDataObjectTechnicalMetadata TechnicalMetadata { get; set; }
            public object DigitalProvenance { get; set; }
        }

        public class PodDataObjectAssignment
        {
            public string Unit { get; set; }
            public string GroupKey { get; set; }
            public string Picklist { get; set; }
            public string Spreadsheet { get; set; }
        }

        public class PodDataObjectBasics
        {
            public string Format { get; set; }
            public int Files { get; set; }
        }
    }

    public class PodDataObjectDetails
    {
        public int Id { get; set; }
        public string BinId { get; set; }
        public string BoxId { get; set; }
        public string PicklistId { get; set; }
        public string ContainerId { get; set; }
        public string Title { get; set; }
        public string TitleControlNumber { get; set; }
        public string HomeLocation { get; set; }
        public string CallNumber { get; set; }
        public string IucatBarcode { get; set; }
        public string Format { get; set; }
        public string CollectionIdentifier { get; set; }
        public string MdpiBarcode { get; set; }
        public string FormatDuration { get; set; }
        public bool HasEphemera { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Author { get; set; }
        public string CatalogKey { get; set; }
        public string CollectionName { get; set; }
        public string Generation { get; set; }
        public string OclcNumber { get; set; }
        public bool OtherCopies { get; set; }
        public int Year { get; set; }
        public string UnitId { get; set; }
        public string GroupKeyId { get; set; }
        public string GroupPosition { get; set; }
        public bool EphemeraReturned { get; set; }
        public string SpreadsheetId { get; set; }
        public string WorkflowStatus { get; set; }
        public int WorkflowIndex { get; set; }
        public bool StagingRequested { get; set; }
        public bool Staged { get; set; }
        public string DigitalStart { get; set; }
        public string StagingRequestTimestamp { get; set; }
        public bool Audio { get; set; }
        public bool Video { get; set; }
        public bool MemnonQcCompleted { get; set; }
        public List<PodDataObjectDetailsWorkflowStatus> WorkflowStatuses { get; set; }
        public string Notes { get; set; }
    }

    public class PodDataObjectDetailsWorkflowStatus
    {
        public int Id { get; set; }
        public string WorkflowStatusTemplateId { get; set; }
        public string PhysicalObjectId { get; set; }
        public string BatchId { get; set; }
        public string BinId { get; set; }
        public string Notes { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string User { get; set; }
        public string HasEphemera { get; set; }
        public string EphemeraReturned { get; set; }
        public string EphemeraOkay { get; set; }
    }

    public class PodDataObjectTechnicalMetadata
    {
        public string PackDeformation { get; set; }
        public string ReelSize { get; set; }
        public string TapeStockBrand { get; set; }
        public int DirectionsRecorded { get; set; }
        public string PreservationProblems { get; set; }
        public string CassetteSize { get; set; }
        public string Coating { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Diameter { get; set; }
        public string Equalization { get; set; }
        public string FormatDuration { get; set; }
        public string FormatVersion { get; set; }
        public string GrooveOrientation { get; set; }
        public string GrooveSize { get; set; }
        public string ImageFormat { get; set; }
        public string Label { get; set; }
        public string Material { get; set; }
        public string RecordingMethod { get; set; }
        public string RecordingStandard { get; set; }

        public PodDataObjectTechnicalMetadataPlaybackSpeed PlaybackSpeed { get; set; }
        public PodDataObjectTechnicalMetadataTrackConfiguration TrackConfiguration { get; set; }
        public PodDataObjectTechnicalMetadataTapeThickness TapeThickness { get; set; }
        public PodDataObjectTechnicalMetadataSoundField SoundField { get; set; }
        public PodDataObjectTechnicalMetadataTapeBase TapeBase { get; set; }
    }

    public class PodDataObjectTechnicalMetadataPlaybackSpeed
    {
        public bool ZeroPoint9375Ips { get; set; }
        public bool OnePoint875Ips { get; set; }
        public bool ThreePoint75Ips { get; set; }
        public bool SevenPoint5Ips { get; set; }
        public bool FifteenIps { get; set; }
        public bool ThirtyIps { get; set; }
        public bool UnknownPlaybackSpeed { get; set; }

        public string GetValue()
        {
            return SevenPoint5Ips ? "7.5 ips" : "Unknown";
        }
    }

    public class PodDataObjectTechnicalMetadataTapeBase
    {
        public bool AcetateBase { get; set; }
    }

    public class PodDataObjectTechnicalMetadataTapeThickness
    {
        public bool OnePoint5Mils { get; set; }
        public bool OneMils { get; set; }

        public string GetValue()
        {
            if (OneMils)
            {
                return "1";
            }

            if (OnePoint5Mils)
            {
                return "1.5";
            }

            return string.Empty;
        }
    }

    public class PodDataObjectTechnicalMetadataSoundField
    {
        public bool Mono { get; set; }
        public bool Stereo { get; set; }

        public string GetValue()
        {
            if (Mono)
            {
                return "Mono";
            }

            if (Stereo)
            {
                return "Stereo";
            }

            return "Unknown";
        }
    }

    public class PodDataObjectTechnicalMetadataTrackConfiguration
    {
        public bool FullTrack { get; set; }
        public bool HalfTrack { get; set; }

        public string GetValue()
        {
            if (FullTrack)
            {
                return "Full track";
            }

            if (HalfTrack)
            {
                return "Half track";
            }

            return "Unknown";
        }
    }
}