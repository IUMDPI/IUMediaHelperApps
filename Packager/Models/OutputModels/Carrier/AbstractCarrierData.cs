using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels.Carrier
{
    [Serializable]
    [XmlInclude(typeof (VideoCarrier))]
    [XmlInclude(typeof (AudioCarrier))]
    public abstract class AbstractCarrierData
    {
    }
}