using System;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class BakingData
    {
        public string  Date { get; set; }

        public bool ShouldSerializeDate()
        {
            return string.IsNullOrWhiteSpace(Date) == false;
        }
    }
}