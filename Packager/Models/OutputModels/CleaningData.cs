using System;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class CleaningData
    {
        public string Date { get; set; }
        public string Comment { get; set; }

        public bool ShouldSerializeDate()
        {
            return string.IsNullOrWhiteSpace(Date) == false;
        }

        public bool ShouldSerializeComment()
        {
            return string.IsNullOrWhiteSpace(Comment) ==false;
        }
    }


}