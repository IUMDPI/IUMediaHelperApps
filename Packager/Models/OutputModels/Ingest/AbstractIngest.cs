using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels.Ingest
{
    [Serializable]
    [XmlInclude(typeof (VideoIngest))]
    [XmlInclude(typeof (AudioIngest))]
    public class AbstractIngest
    {
       
    }
}