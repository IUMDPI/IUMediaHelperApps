using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packager.Utilities
{
    interface IUtilityProvider
    {
        IExcelImporter ExcelImporter { get; }
        IBextDataProvider BextDataProvider { get; }
        IHasher Hasher { get; }
        IUserInfoResolver UserInfoResolver { get; }
        IXmlExporter XmlExporter { get; }
    }
}
