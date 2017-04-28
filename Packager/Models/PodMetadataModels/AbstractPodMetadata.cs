using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Common.Models;
using Packager.Extensions;
using Packager.Factories;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public abstract class AbstractPodMetadata : IImportable
    {
        [Required]
        public IMediaFormat Format { get; set; }
        public string CallNumber { get; set; }
        public string Title { get; set; }
        [Required]
        public string Unit { get; set; }
        [Required]
        public string Barcode { get; set; }
        public DateTime? CleaningDate { get; set; }
        public string CleaningComment { get; set; }
        public DateTime? BakingDate { get; set; }
        public string Repaired { get; set; }
        public string PlaybackSpeed { get; set; }
       
        public string Damage { get; set; }
        public string PreservationProblems { get; set; }
        public List<AbstractDigitalFile> FileProvenances { get; set; }
        public string Comments { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        [Required]
        public string Iarl { get; set; }
        [Required]
        public string Icmt { get; set; }
        [Required]
        public string Bext { get; set; }

        public virtual void ImportFromXml(XElement element, IImportableFactory factory)
        {
            Success = factory.ToBooleanValue(element, "success");
            Message = factory.ToStringValue(element, "message");

            Comments = factory.ToStringValue(element, "data/comments");
            Format = MediaFormats.GetFormat(factory.ToStringValue(element, "data/format"));
            CallNumber = factory.ToStringValue(element, "data/call_number");
            Title = factory.ToStringValue(element, "data/title");
            Unit = factory.ToStringValue(element, "data/unit");
            Barcode = factory.ToStringValue(element, "data/mdpi_barcode");
            BakingDate = factory.ToDateTimeValue(element, "data/baking_date");
            CleaningDate = factory.ToDateTimeValue(element, "data/cleaning_date");
            CleaningComment = factory.ToStringValue(element, "data/cleaning_comment");
            Repaired = factory.ToBooleanValue(element, "data/repaired").ToYesNo();
            Damage = factory.ToStringValue(element, "data/damage").ToDefaultIfEmpty("None");
            PreservationProblems = factory.ToStringValue(element, "data/preservation_problems");
            PlaybackSpeed = factory.ToStringValue(element, "data/playback_speed");
            Iarl = factory.ToStringValue(element, "data/file_iarl");
            Icmt = factory.ToStringValue(element, "data/file_icmt");
            Bext = factory.ToStringValue(element, "data/file_bext");

            // process each digital_file_provenance node
            FileProvenances = ImportFileProvenances(element, "data/digital_files/digital_file_provenance", factory);
        }

        // method is abstract; implemented in derived classes
        protected abstract List<AbstractDigitalFile> ImportFileProvenances(XElement element, string path,
            IImportableFactory factory);
    }
}