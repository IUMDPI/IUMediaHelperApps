using System;
using Packager.Extensions;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class BakingData
    {
        public string  Date { get; set; }

        public bool ShouldSerializeDate()
        {
            return Date.IsSet();
        }
    }
}