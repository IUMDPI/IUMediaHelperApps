using System.Xml.Linq;
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
            SpeedUsed = factory.ToStringValue(element, "speed_used");
            ReferenceFluxivity = factory.ToStringValue(element, "tape_fluxivity");
            AnalogOutputVoltage = factory.ToStringValue(element, "analog_output_voltage");
            Peak = factory.ToStringValue(element, "peak");
            StylusSize = factory.ToStringValue(element, "stylus_size");
            Turnover = factory.ToStringValue(element, "turnover");
            Gain = factory.ToStringValue(element, "volume_units");
            Rolloff = factory.ToStringValue(element, "rolloff");
        }
    }
}