using System;
using System.Linq;
using System.Threading.Tasks;
using Packager.Providers;

namespace Packager.Utilities
{
    public interface ISuccessFolderCleaner
    {
        Task DoCleaning();
    }

    public class SuccessFolderCleaner : ISuccessFolderCleaner
    {
        public SuccessFolderCleaner(IDirectoryProvider directoryProvider, string successFolder, TimeSpan interval)
        {
            DirectoryProvider = directoryProvider;
            SuccessFolder = successFolder;
            Interval = interval;
        }

        private IDirectoryProvider DirectoryProvider { get; }
        private string SuccessFolder { get; }
        private TimeSpan Interval { get; }

        public async Task DoCleaning()
        {
            if (Interval.Days < 1)
            {
                return;
            }

            var targets = DirectoryProvider.EnumerateDirectories(SuccessFolder)
                .Where(i => i.CreationTime < DateTime.Now.Subtract(Interval)).ToList();

            if (!targets.Any())
            {
                return;
            }

            foreach (var target in targets)
            {
                await DirectoryProvider.DeleteDirectoryAsync(target.FullName);
            }
        }
    }
}