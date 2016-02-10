using System;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class CleaningData
    {
        public DateTime? Date { get; set; }
        public string Comment { get; set; }

        public bool ShouldSerializeDate()
        {
            return Date.HasValue;
        }

        public bool ShouldSerializeComment()
        {
            return string.IsNullOrWhiteSpace(Comment) ==false;
        }
    }


}