using System;

namespace Packager.Models.UserInterfaceModels
{
    public class SectionModel
    {
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }
        public string Key { get; set; }
        public bool Completed { get; set; }
        public int Indent { get; set; }
        public string Title { get; set; }
        
    }
}