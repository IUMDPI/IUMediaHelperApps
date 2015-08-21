using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Observers;
using Packager.Providers;

namespace Packager.Utilities
{
    public interface ISuccessFolderCleaner
    {
        bool Enabled { get; }
        string ConfiguredInterval { get; }
        Task DoCleaning();
    }

    public class SuccessFolderCleaner : ISuccessFolderCleaner
    {
        public SuccessFolderCleaner(IDirectoryProvider directoryProvider, string successFolder, TimeSpan interval, IObserverCollection observers)
        {
            DirectoryProvider = directoryProvider;
            SuccessFolder = successFolder;
            Observers = observers;
            Interval = interval;
        }

        private IDirectoryProvider DirectoryProvider { get; }
        private string SuccessFolder { get; }
        private IObserverCollection Observers { get; }
        private TimeSpan Interval { get; }

        public async Task DoCleaning()
        {
            if (Enabled == false)
            {
                return;
            }

            var targets = DirectoryProvider.GetFolderNamesAndCreationDates(SuccessFolder)
                .Where(i => i.Value < DateTime.Now.Subtract(Interval)).ToList();

            if (!targets.Any())
            {
                return;
            }

            var section = Observers.BeginSection("Removing {0} from success folder",
                targets.ToSingularOrPlural("item", "items"));

            foreach (var target in targets)
            {
                await RemoveFolder(target.Key);
            }
            Observers.EndSection(section);
        }

        public bool Enabled => Interval.Days > 0;

        public string ConfiguredInterval => $"{Interval.Days.ToSingularOrPlural("day", "days")}";

        private async Task RemoveFolder(string folderName)
        {
            try
            {
                await DirectoryProvider.DeleteDirectoryAsync(Path.Combine(SuccessFolder, folderName));
                Observers.Log("{0} removed", folderName);
            }
            catch (Exception e)
            {
                Observers.Log("Could not remove {0}: {1}", folderName, e.Message.ToDefaultIfEmpty("[unknown issue]"));
            }
        }
    }
}