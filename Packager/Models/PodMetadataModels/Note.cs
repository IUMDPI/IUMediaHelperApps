namespace Packager.Models.PodMetadataModels
{
    public class Note
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string User { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public bool Export { get; set; }
    }
}