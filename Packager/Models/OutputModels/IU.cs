using System;
using System.Xml.Serialization;
using Packager.Models.OutputModels.Carrier;

namespace Packager.Models.OutputModels
{
    [Serializable]
    [XmlRoot(ElementName = "IU")]
    public class IU
    {
        public AbstractCarrierData Carrier { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "IU")]
    public class IU<T> where T: AbstractCarrierData
    {
        public T Carrier { get; set; }
    }
    
}