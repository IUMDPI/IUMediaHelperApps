using System.Xml.Linq;
using Packager.Extensions;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class DigitalAudioFile : AbstractDigitalFile
    {
        [Required]
        public string SpeedUsed { get; set; }
        
        public override void ImportFromXml(XElement element)
        {
            base.ImportFromXml(element);
            SpeedUsed = element.ToStringValue("speed_used");
        }
    }
}