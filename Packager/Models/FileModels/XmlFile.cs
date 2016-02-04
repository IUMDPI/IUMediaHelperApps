using System;
using System.Linq;

namespace Packager.Models.FileModels
{
    public class XmlFile : AbstractFile
    {
        public XmlFile(string projectCode, string barCode)
        {
            ProjectCode = projectCode;
            BarCode = barCode;
        }

        public override string FileUse => "";
        public override string FullFileUse => "";
        public override string Extension => ".xml";

        public override bool IsSameAs(AbstractFile model)
        {
            throw new NotImplementedException();
        }

        public override bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ProjectCode))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(BarCode))
            {
                return false;
            }

            return true;
        }

        public override string ToFileName()
        {
            var parts = new[] {ProjectCode, BarCode};

            var fileName = string.Join("_", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
            return $"{fileName}{Extension}";
        }
    }
}