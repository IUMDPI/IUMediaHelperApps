using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packager.Models.FileModels
{
    class XmlFileModel:AbstractFileModel
    {
        public override string ToFileName()
        {
            var parts = new[] { ProjectCode, BarCode };

            var fileName = string.Join("_", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
            return string.Format("{0}{1}", fileName, Extension);
        }
    }
}
