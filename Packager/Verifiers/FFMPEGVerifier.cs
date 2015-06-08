namespace Packager.Verifiers
{
    public class FFMPEGVerifier : IVerifier
    {
        public FFMPEGVerifier(int exitCode)
        {
            ExitCode = exitCode;
        }

        private int ExitCode { get; set; }

        public bool Verify()
        {
            return ExitCode == 0;
        }
    }
}