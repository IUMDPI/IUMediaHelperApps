namespace Packager.Utilities.Process
{
    public interface IOutputBuffer
    {
        void AppendLine(string value);
        string GetContent();
    }
}
