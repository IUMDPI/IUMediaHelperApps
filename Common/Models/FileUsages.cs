using System.Collections.Generic;

namespace Common.Models
{
    public class FileUsages
    {
        protected FileUsages(string fileUse, string fullFileUse)
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

        private static Dictionary<string , FileUsages> AllUsages = new Dictionary<string, FileUsages>
        {
            { PreservationMaster.FileUse.ToLowerInvariant(), PreservationMaster},
            { PreservationIntermediateMaster.FileUse.ToLowerInvariant(), PreservationIntermediateMaster },
            {ProductionMaster.FileUse.ToLowerInvariant(), ProductionMaster },
            {MezzanineFile.FileUse.ToLowerInvariant(), MezzanineFile },
            {AccessFile.FileUse.ToLowerInvariant(), AccessFile },
            {LabelImageFile.FileUse.ToLowerInvariant(), LabelImageFile }
        };

        public static FileUsages GetUsage(string fileUse)
        {
            return AllUsages.ContainsKey(fileUse.ToLowerInvariant()) 
                ? AllUsages[fileUse.ToLowerInvariant()] 
                : null;
        }
    }
}
