using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class IU
    {
        public AbstractCarrierData Carrier { get; set; }
    }

    
}