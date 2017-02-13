namespace Packager.UserInterface
{
    public interface IViewModel
    {
        void Initialize(OutputWindow outputWindow, string projectCode);
        bool Processing { get; set; }
        string ProcessingMessage { get; set; }
    }
}