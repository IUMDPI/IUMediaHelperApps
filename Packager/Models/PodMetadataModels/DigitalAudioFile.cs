using System.Xml.Linq;
using Packager.Extensions;
using Packager.Factories;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class DigitalAudioFile : AbstractDigitalFile
    {
        [Required]
        public string SpeedUsed { get; set; }

        public string ReferenceFluxivity { get; set; }
        public string AnalogOutputVoltage { get; set; }
        public string Peak { get; set; }
        public string StylusSize { get; set; }
        public string Turnover { get; set; }
        public string Gain { get; set; }
        public string Rolloff { get; set; }

        // xpath queries here are against each digital_file_provenance node in
        // digital_files
        public override void ImportFromXml(XElement element, IImportableFactory factory)
        {
            base.ImportFromXml(element, factory);
            SpeedUsed = element.ToStringValue("speed_used");
            ReferenceFluxivity = element.ToStringValue("tape_fluxivity").AppendIfValuePresent(" nWb/m");
            AnalogOutputVoltage = element.ToStringValue("analog_output_voltage").AppendIfValuePresent(" dBu");
            Peak = element.ToStringValue("peak").AppendIfValuePresent(" dBfs");
            StylusSize = element.ToStringValue("stylus_size");
            Turnover = element.ToStringValue("turnover");
            Gain = element.ToStringValue("volume_units").AppendIfValuePresent(" dB");
            Rolloff = element.ToStringValue("rolloff");
        }
    }
}