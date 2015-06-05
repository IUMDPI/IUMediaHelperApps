using System;
using System.Collections.Generic;

namespace Packager.Models.PodMetadataModels
{
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
        //public object ConditionStatuses { get; set; }
    }
}