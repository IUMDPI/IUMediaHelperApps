using System.Data;

namespace Packager.Utilities
{
    public interface IExcelImporter
    {
        object Import(DataRow row);
    }
}