using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Models
{
    public interface IFileUsage
    {
        string FileUse { get; }
        string FullFileUse { get; }
        int Precedence { get; }
    }

    public class FileUsage : IFileUsage
    {
        internal FileUsage(string fileUse, string fullFileUse, int precendence)
        {
            FileUse = fileUse;
            FullFileUse = fullFileUse;
            Precedence = precendence;
        }

        public string FileUse { get; }
        public string FullFileUse { get; }
        public int Precedence { get; }
    }

    public class QualityControlFileUsage : IFileUsage
    {
        public QualityControlFileUsage(IFileUsage originalUsage)
        {
            FileUse = originalUsage.FileUse;
            FullFileUse = originalUsage.FullFileUse;
        }

        public string FileUse { get; }
        public string FullFileUse { get; }
        public int Precedence => 8;
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
        public int Precedence => 100;
    }

    public class FileUsages
    {
        public static readonly IFileUsage PreservationMaster = new FileUsage("pres", "Preservation Master",0);
        public static readonly IFileUsage PreservationToneReference = new FileUsage("presRef", "Preservation Master Tone Reference File",1);
        public static readonly IFileUsage PreservationIntermediateMaster = new FileUsage("presInt", "Preservation Master - Intermediate",2);
        public static readonly IFileUsage PreservationIntermediateToneReference = new FileUsage("intRef",
            "Preservation Master - Intermediate Tone Reference File",3);
        public static readonly IFileUsage ProductionMaster = new FileUsage("prod", "Production Master",4);
        public static readonly IFileUsage MezzanineFile = new FileUsage("mezz", "Mezzanine File",5);
        public static readonly IFileUsage AccessFile = new FileUsage("access", "Access File",6);
        public static readonly IFileUsage LabelImageFile = new FileUsage("label", "Label Image File",7);
        public static readonly IFileUsage XmlFile = new FileUsage("", "Xml File",100);
        
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

        public static bool IsImportable(IFileUsage usage)
        {
            return AllImportableUsages.Contains(usage);
        }
    }
}
