using System;
using System.Xml.Serialization;
using Packager.Models.OutputModels.Carrier;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class IU
    {
        public AbstractCarrierData Carrier { get; set; }
    }

    
}