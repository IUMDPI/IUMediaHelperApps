using System;

namespace Packager.Models.PodMetadataModels
{
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
}