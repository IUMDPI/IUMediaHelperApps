namespace Packager.Models.PodMetadataModels
{
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
}