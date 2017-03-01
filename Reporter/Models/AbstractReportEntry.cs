namespace Reporter.Models
{
    public abstract class AbstractReportEntry
    {
        public string DisplayName { get; set; }
        public long Timestamp { get; set; }
    }
}