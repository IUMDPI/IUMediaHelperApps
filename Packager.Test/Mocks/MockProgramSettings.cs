using NSubstitute;
using Packager.Models;

namespace Packager.Test.Mocks
{
    public static class MockProgramSettings
    {
        public const string ProjectCode = "mdpi";
        public const string InputDirectory = @"c:\work\mdpi";
        public const string ProcessingDirectory = @"c:\work\processing";
        public const string DropBoxDirectory = @"c:\work\dropbox";

        public static IProgramSettings Get(
            string projectCode = ProjectCode,
            string inputDirectory = InputDirectory,
            string processingDirectory = ProcessingDirectory,
            string dropBoxDirectory = DropBoxDirectory
            )
        {
            var result = Substitute.For<IProgramSettings>();
            result.ProjectCode.Returns(projectCode);
            result.InputDirectory.Returns(inputDirectory);
            result.ProcessingDirectory.Returns(processingDirectory);
            result.DropBoxDirectoryName.Returns(dropBoxDirectory);
            return result;
        }
    }
}