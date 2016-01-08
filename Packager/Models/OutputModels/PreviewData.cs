using System;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class PreviewData
    {
        public string Comments { get; set; }

        public bool ShouldSerializeComments()
        {
            return string.IsNullOrWhiteSpace(Comments) == false;
        }
    }
}