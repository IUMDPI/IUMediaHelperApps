using System;
using Packager.Extensions;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class CleaningData
    {
        public string Date { get; set; }
        public string Comment { get; set; }

        public bool ShouldSerializeDate()
        {
            return Date.IsSet();
        }

        public bool ShouldSerializeComment()
        {
            return Comment.IsSet();
        }
    }


}