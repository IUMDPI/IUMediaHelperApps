using System.Xml.Linq;
using Packager.Extensions;
using Packager.Providers;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class DigitalAudioFile : AbstractDigitalFile
    {
        [Required]
        public string SpeedUsed { get; set; }
        
        public override void ImportFromXml(XElement element, ILookupsProvider lookupsProvider)
        {
            base.ImportFromXml(element, lookupsProvider);
            SpeedUsed = element.ToStringValue("speed_used");
        }
    }
}