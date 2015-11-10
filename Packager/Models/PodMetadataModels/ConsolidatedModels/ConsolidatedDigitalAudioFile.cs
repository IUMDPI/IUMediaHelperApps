using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels.ConsolidatedModels
{
    public class ConsolidatedDigitalAudioFile : AbstractConsolidatedDigitalFile
    {
        [Required]
        public string SpeedUsed { get; set; }

        public ConsolidatedDigitalAudioFile(DigitalFileProvenance original) : base(original)
        {
            SpeedUsed = original.SpeedUsed;
        }
    }
}