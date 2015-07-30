using System.Collections.Generic;
using System.ComponentModel;

namespace Packager.Models.PodMetadataModels
{
    public class PodMetadata
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public DataObject Object { get; set; }
    }

    public class DataObject
    {
        public Basics Basics { get; set; }
        public Assignment Assignment { get; set; }
        public Details Details { get; set; }
        public TechnicalMetadata TechnicalMetadata { get; set; }
        public DigitalProvenance DigitalProvenance { get; set; }
    }

    public class Basics
    {
        public string Format { get; set; }
        public int Files { get; set; }
    }

    public class Assignment
    {
        public string Unit { get; set; }
        public string GroupKey { get; set; }
        public string Picklist { get; set; }
        public int Box { get; set; }
        public int Bin { get; set; }
        public string Batch { get; set; }
        public string Spreadsheet { get; set; }
    }

    public class Details
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TitleControlNumber { get; set; }
        public string HomeLocation { get; set; }
        public string CallNumber { get; set; }
        public string IucatBarcode { get; set; }
        public string Format { get; set; }
        public string CollectionIdentifier { get; set; }
        public string MdpiBarcode { get; set; }
        public bool HasEphemera { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Author { get; set; }
        public string CatalogKey { get; set; }
        public string CollectionName { get; set; }
        public string Generation { get; set; }
        public string OclcNumber { get; set; }
        public bool OtherCopies { get; set; }
        public string Year { get; set; }
        public string GroupPosition { get; set; }
        public bool EphemeraReturned { get; set; }
        public bool StagingRequested { get; set; }
        public bool Staged { get; set; }
        public string DigitalStart { get; set; }
        public string StagingRequestTimestamp { get; set; }
        public bool Audio { get; set; }
        public bool Video { get; set; }
        public bool MemnonQcCompleted { get; set; }
        public int GroupTotal { get; set; }
        public string CarrierStreamIndex { get; set; }
        public string CurrentWorkflowStatus { get; set; }
        public List<WorkflowStatus> WorkflowStatuses { get; set; }
        public List<Note> Notes { get; set; }
        public List<ConditionStatus> ConditionStatuses { get; set; }
    }

    public class WorkflowStatus
    {
        public string Id { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string User { get; set; }
        public bool HasEphemera { get; set; }
        public bool EphemeraReturned { get; set; }
        public bool EphemeraOkay { get; set; }
        public string Name { get; set; }
        public string SequenceIndex { get; set; }
        public string WorkflowNote { get; set; }
    }

    public class Note
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string User { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public bool Export { get; set; }
    }

    public class ConditionStatus
    {
        public string Id { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string User { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public bool BlocksPacking { get; set; }
        public string ConditionNote { get; set; }
    }

    public class TechnicalMetadata
    {
        public string Format { get; set; }
        public int Files { get; set; }
        public Damage Damage { get; set; }
        public PlaybackSpeed PlaybackSpeed { get; set; }
        public PreservationProblems PreservationProblems { get; set; }
        public SampleRate SampleRate { get; set; }
        public SoundField SoundField { get; set; }
        public TapeBase TapeBase { get; set; }
        public TrackConfiguration TrackConfiguration { get; set; }
        public TapeThickness TapeThickness { get; set; }
        public string CassetteSize { get; set; }
        public string Coating { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Diameter { get; set; }
        public string DirectionsRecorded { get; set; }
        public string Equalization { get; set; }
        public string FormatDuration { get; set; }
        public string FormatVersion { get; set; }
        public string GrooveOrientation { get; set; }
        public string GrooveSize { get; set; }
        public string ImageFormat { get; set; }
        public string Label { get; set; }
        public string Material { get; set; }
        public string PackDeformation { get; set; }
        public string RecordingMethod { get; set; }
        public string RecordingStandard { get; set; }
        public string ReelSize { get; set; }
        public string Speed { get; set; }
        public string Substrate { get; set; }
        public string TapeStockBrand { get; set; }
    }

    public class Damage
    {
        public bool Broken { get; set; }
        public bool Cracked { get; set; }
        public bool Dirty { get; set; }
        public bool Fungus { get; set; }
        public bool Scratched { get; set; }
        public bool Warped { get; set; }
        public bool Worn { get; set; }
    }

    public class PlaybackSpeed
    {
        [Description(".9375 ips")]
        public bool ZeroPoint9375Ips { get; set; }

        [Description("1.875 ips")]
        public bool OnePoint875Ips { get; set; }

        [Description("3.75 ips")]
        public bool ThreePoint75Ips { get; set; }
        
        [Description("7.5 ips")]
        public bool SevenPoint5Ips { get; set; }

        [Description("15 ips")]
        public bool FifteenIps { get; set; }

        [Description("30 ips")]
        public bool ThirtyIps { get; set; }

        [Description("Unknown")]
        public bool UnknownPlaybackSpeed { get; set; }
    }

    public class PreservationProblems
    {
        [Description("Breakdown of materials")]
        public bool BreakdownOfMaterials { get; set; }

        [Description("Delamination")]
        public bool Delamination { get; set; }

        [Description("Exudation")]
        public bool Exudation { get; set; }

        [Description("Fungus")]
        public bool Fungus { get; set; }

        [Description("Other contaminants")]
        public bool OtherContaminants { get; set; }

        [Description("Oxidation")]
        public bool Oxidation { get; set; }

        [Description("Soft binder syndrome")]
        public bool SoftBinderSyndrome { get; set; }

        [Description("Vinegar syndrome")]
        public bool VinegarSyndrome { get; set; }
    }

    public class SampleRate
    {
        [Description("32k")]
        public bool SampleRate32K { get; set; }

        [Description("44.1k")]
        public bool SampleRate441K { get; set; }

        [Description("48k")]
        public bool SampleRate48K { get; set; }

        [Description("96k")]
        public bool SampleRate96K { get; set; }
    }

    public class SoundField
    {
        [Description("Mono")]
        public bool Mono { get; set; }

        [Description("Stereo")]
        public bool Stereo { get; set; }

        [Description("Unknown")]
        public bool UnknownSoundField { get; set; }
    }

    public class TapeBase
    {
        public bool AcetateBase { get; set; }
        public bool PolyesterBase { get; set; }
        public bool PvcBase { get; set; }
        public bool PaperBase { get; set; }
    }

    public class TapeThickness
    {
        
        [Description(".5")]
        public bool ZeroPoint5Mils { get; set; }

        [Description("1.0")]
        public bool OneMils { get; set; }

        [Description("1.5")]
        public bool OnePoint5Mils { get; set; }
    }

    public class TrackConfiguration
    {
        [Description("Full track")]
        public bool FullTrack { get; set; }

        [Description("Full track")]
        public bool HalfTrack { get; set; }

        [Description("Full track")]
        public bool QuarterTrack { get; set; }

        [Description("Unknown")]
        public bool UnknownTrack { get; set; }
    }


    public class DigitalProvenance
    {
        public string DigitizingEntity { get; set; }
        public string Comments { get; set; }
        public string CleaningDate { get; set; }
        public string Baking { get; set; }
        public bool Repaired { get; set; }
        public string CleaningComment { get; set; }
        public string Duration { get; set; }
        public List<DigitalFileProvenance> DigitalFileProvenances { get; set; }
    }

    public class DigitalFileProvenance
    {
        public string DateDigitized { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public string PlayerSerialNumber { get; set; }
        public string PlayerManufacturer { get; set; }
        public string PlayerModel { get; set; }
        public string AdSerialNumber { get; set; }
        public string AdManufacturer { get; set; }
        public string AdModel { get; set; }
        public string ExtractionWorkstation { get; set; }
        public string SpeedUsed { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Filename { get; set; }
    }
}