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
        public string TapeFluxivity { get; set; }
        public string AnalogOutputVoltage { get; set; }
        public string Peak { get; set; }
        public string StylusSize { get; set; }
        public string TurnOver { get; set; }

        public override void ImportFromXml(XElement element, ILookupsProvider lookupsProvider)
        {
            base.ImportFromXml(element, lookupsProvider);
            SpeedUsed = element.ToStringValue("speed_used");
            TapeFluxivity = element.ToStringValue("tape_fluxivity");
            AnalogOutputVoltage = element.ToStringValue("analog_output_voltage");
            Peak = element.ToStringValue("peak");
            StylusSize = element.ToStringValue("stylus_size");
            TurnOver = element.ToStringValue("turnover");
        }
    }
}