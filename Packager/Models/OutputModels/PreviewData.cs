using System;
using Packager.Extensions;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class PreviewData
    {
        public string Comments { get; set; }

        public bool ShouldSerializeComments()
        {
            return Comments.IsSet();
        }
    }
}