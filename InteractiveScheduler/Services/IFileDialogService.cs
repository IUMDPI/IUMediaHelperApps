namespace InteractiveScheduler.Services
{
    public interface IFileDialogService
    {
        string OpenDialog(string defaultPath = "");
        bool Exists(string path);
    }
}