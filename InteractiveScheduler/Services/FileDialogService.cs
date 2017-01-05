using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace InteractiveScheduler.Services
{
    public class FileDialogService : IFileDialogService
    {
        private readonly Window _owner;

        public FileDialogService(Window owner)
        {
            _owner = owner;
        }

        public string OpenDialog(string defaultPath = "")
        {
            var initialDirectory = string.IsNullOrWhiteSpace(defaultPath)
                ? string.Empty
                : Path.GetDirectoryName(defaultPath);

            var dialog = new OpenFileDialog
            {
                Filter = "exe files (*.exe) | *.exe",
                CheckPathExists = true,
                CheckFileExists = true,
                InitialDirectory = initialDirectory,
                Multiselect = false
            };

            var result = dialog.ShowDialog(_owner);
            return result.Value ? dialog.FileName : string.Empty;
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}
