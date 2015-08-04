namespace Packager.Verifiers
{
    public interface IFFMPEGVerifier
    {
        bool Verify(int exitCode);
    }

    public class FFMPEGVerifier : IFFMPEGVerifier
    {
        public bool Verify(int exitCode)
        {
            return exitCode == 0;
        }
    }
}