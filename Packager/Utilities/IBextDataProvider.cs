using Packager.Models;

namespace Packager.Utilities
{
    public interface IBextDataProvider
    {
        BextData GetMetadata(string barcode);
    }
}