﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Models
{
    public interface IFileUsage
    {
        string FileUse { get; }
        string FullFileUse { get; }
    }

    public class FileUsage : IFileUsage
    {
        internal FileUsage(string fileUse, string fullFileUse)
        {
            FileUse = fileUse;
            FullFileUse = fullFileUse;
        }

        public string FileUse { get; }
        public string FullFileUse { get; }
    }

    public class UnknownFileUsage: IFileUsage
    {
        public UnknownFileUsage(string fileUse, string fullFileUse)
        {
            FileUse = fileUse;
            FullFileUse = fullFileUse;
        }
        public string FileUse { get; }
        public string FullFileUse { get; }
    }

    public class FileUsages
    {
        public static readonly IFileUsage PreservationMaster = new FileUsage("pres", "Preservation Master");
        public static readonly IFileUsage PreservationIntermediateMaster = new FileUsage("presInt", "Preservation Master - Intermediate");
        public static readonly IFileUsage ProductionMaster = new FileUsage("prod", "Production Master");
        public static readonly IFileUsage MezzanineFile = new FileUsage("mezz", "Mezzanine File");
        public static readonly IFileUsage AccessFile = new FileUsage("access", "Access File");
        public static readonly IFileUsage LabelImageFile = new FileUsage("label", "Label Image File");
        public static readonly IFileUsage XmlFile = new FileUsage("", "Xml File");
        public static readonly IFileUsage PreservationToneReference = new FileUsage("presRef", "Preservation Master Tone Reference File");
        public static readonly IFileUsage PreservationIntermediateToneReference = new FileUsage("intRef",
            "Preservation Master - Intermediate Tone Reference File");
       
        private static readonly List<IFileUsage> AllImportableUsages = new List<IFileUsage>
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

        public static IFileUsage GetUsage(string fileUse)
        {
            var usage = AllImportableUsages.SingleOrDefault(
                    u => u.FileUse.Equals(fileUse, StringComparison.InvariantCultureIgnoreCase));

            return usage ??new UnknownFileUsage(fileUse, "Raw object file");
        }
    }
}
