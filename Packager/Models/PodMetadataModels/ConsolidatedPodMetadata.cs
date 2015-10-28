﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class ConsolidatedPodMetadata
    {
        private const string NotSetText = "[not set]";

        [Required]
        public string Identifier { get; set; }

        [Required]
        public string Format { get; set; }

        [Required]
        public string CallNumber { get; set; }

        public string Title { get; set; }

        [Required]
        public string Unit { get; set; }

        [Required]
        public string Barcode { get; set; }

        public string Brand { get; set; }
        public string DirectionsRecorded { get; set; }
        public DateTime? CleaningDate { get; set; }
        public string CleaningComment { get; set; }
        public DateTime? BakingDate { get; set; }
        public string Repaired { get; set; }

        [Required]
        public string DigitizingEntity { get; set; }

        public string PlaybackSpeed { get; set; }

        public string TrackConfiguration { get; set; }

        [Required]
        public string SoundField { get; set; }

        public string TapeThickness { get; set; }
        public List<DigitalFileProvenance> FileProvenances { get; set; }
        public string Damage { get; set; }
        public string PreservationProblems { get; set; }
    }
}