using System.Windows.Media;

namespace Recorder.ViewModels
{
    public interface IWindowHandleInitialized
    {
        void WindowHandleInitialized(Visual client);
    }
}