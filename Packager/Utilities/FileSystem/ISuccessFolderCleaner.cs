using System.Threading.Tasks;

namespace Packager.Utilities.FileSystem
{
    public interface ISuccessFolderCleaner
    {
        bool Enabled { get; }
        string ConfiguredInterval { get; }
        Task DoCleaning();
    }
}