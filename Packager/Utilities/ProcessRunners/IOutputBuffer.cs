namespace Packager.Utilities.ProcessRunners
{
    public interface IOutputBuffer
    {
        void AppendLine(string value);
        string GetContent();
    }
}
