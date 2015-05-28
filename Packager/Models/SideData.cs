using System.Collections.Generic;

namespace Packager.Models
{
    public class SideData
    {
        public string Side { get; set; }
        public IngestData Ingest { get; set; }
        public string ManualCheck { get; set; }
        public List<FileData> Files { get; set; }
    }
}