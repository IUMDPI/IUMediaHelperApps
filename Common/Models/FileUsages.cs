using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Models
{
    public class FileUsages
    {
        private FileUsages(string fileUse, string fullFileUse)
        {
            FileUse = fileUse;
            FullFileUse = fullFileUse;
        }

        public string FileUse { get; }
        public string FullFileUse { get; }

        public static readonly FileUsages PreservationMaster = new FileUsages("pres", "Preservation Master");
        public static readonly FileUsages PreservationIntermediateMaster = new FileUsages("presInt", "Preservation Master - Intermediate");
        public static readonly FileUsages ProductionMaster = new FileUsages("prod", "Production Master");
        public static readonly FileUsages MezzanineFile = new FileUsages("mezz", "Mezzanine File");
        public static readonly FileUsages AccessFile = new FileUsages("access", "Access File");
        public static readonly FileUsages LabelImageFile = new FileUsages("label", "Label Image File");
        public static readonly FileUsages XmlFile = new FileUsages("", "Xml File");
        public static readonly FileUsages UnknownFile = new FileUsages("", "Raw object file");
        public static readonly FileUsages PreservationToneReference = new FileUsages("presRef", "Preservation Master Tone Reference File");
        public static readonly FileUsages PreservationIntermediateToneReference = new FileUsages("intRef", "Preservation Master - Intermediate Tone Reference File");

        private static readonly List<FileUsages> AllImportableUsages = new List< FileUsages>
        {
            PreservationMaster,
            PreservationIntermediateMaster,
            PreservationToneReference,
            PreservationIntermediateToneReference,
            ProductionMaster,
            MezzanineFile,
            AccessFile,
            LabelImageFile
        };

        public static FileUsages GetUsage(string fileUse)
        {
            var usage = AllImportableUsages.SingleOrDefault(
                    u => u.FileUse.Equals(fileUse, StringComparison.InvariantCultureIgnoreCase));

            return usage ?? UnknownFile;
        }
    }
}
