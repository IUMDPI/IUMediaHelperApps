using System;
using System.Xml.Serialization;
using Common.Extensions;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class File
    {
        public File()
        {
            PlaceHolder = string.Empty;     // ensures that placeholder node gets 
                                            // serialized as <PlaceHolder />
        }

        [XmlElement("FileName", Order = 1)]
        public string FileName { get; set; }

        [XmlElement("CheckSum", Order =2)]
        public string Checksum { get; set; }

        [XmlElement("PlaceHolder", Order = 3)]
        public string PlaceHolder {get;set; }

        // include Checksum node if it is set; otherwise, suppress it
        public bool ShouldSerializeChecksum() => Checksum.IsSet();

        // if checksum node is not set, include PlaceHolder node; 
        // otherwise suppress PlaceHolder node
        public bool ShouldSerializePlaceHolder() => Checksum.IsNotSet();    
    }
}