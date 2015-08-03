namespace Packager.Models.PodMetadataModels
{
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
}