using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NSubstitute;
using Packager.Models;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities;
using Packager.Utilities.Hashing;
using Packager.Utilities.Process;
using Packager.Utilities.Xml;
using Packager.Validators;

namespace Packager.Test.Mocks
{
    public static class MockDependencyProvider
    {
        public static IDependencyProvider Get(
            IDirectoryProvider directoryProvider = null,
            IFileProvider fileProvider = null,
            IHasher hasher = null,
            IProcessRunner processRunner = null,
            IProgramSettings programSettings = null,
            IXmlExporter xmlExporter = null,
            IObserverCollection observers = null,
            IValidatorCollection validators = null,
            IFFMPEGRunner ffmpegRunner = null)
        {
            if (directoryProvider == null)
            {
                directoryProvider = Substitute.For<IDirectoryProvider>();
            }

            if (fileProvider == null)
            {
                fileProvider = Substitute.For<IFileProvider>();
            }

            if (hasher == null)
            {
                hasher = Substitute.For<IHasher>();
            }

            if (xmlExporter == null)
            {
                xmlExporter = Substitute.For<IXmlExporter>();
            }

            if (processRunner == null)
            {
                processRunner = MockProcessRunner.Get();
            }

            if (programSettings == null)
            {
                programSettings = MockProgramSettings.Get();
            }

            if (observers == null)
            {
                observers = Substitute.For<IObserverCollection>();
            }

            if (validators == null)
            {
                validators = Substitute.For<IValidatorCollection>();
                validators.Validate(null).ReturnsForAnyArgs(new ValidationResults());
            }

            if (ffmpegRunner == null)
            {
                ffmpegRunner = Substitute.For<IFFMPEGRunner>();
                ffmpegRunner.CreateAccessDerivative(Arg.Any<AbstractFile>())
                    .Returns(a=> Task.FromResult((AbstractFile)new AccessFile(a.Arg<AbstractFile>())));
                    
                ffmpegRunner.CreateProdOrMezzDerivative(Arg.Any<AbstractFile>(), Arg.Any<AbstractFile>(), Arg.Any<EmbeddedAudioMetadata>())
                    .Returns(a => new AccessFile(a.Arg<AbstractFile>()));
            }



            var result = Substitute.For<IDependencyProvider>();

            result.DirectoryProvider.Returns(directoryProvider);
            result.FileProvider.Returns(fileProvider);
            result.Hasher.Returns(hasher);
            result.XmlExporter.Returns(xmlExporter);
            result.ProcessRunner.Returns(processRunner);
            result.ProgramSettings.Returns(programSettings);
            result.Observers.Returns(observers);
            result.ValidatorCollection.Returns(validators);
            result.AudioFFMPEGRunner.Returns(ffmpegRunner);
            return result;
        }

        private static Task<AccessFile> GetFakeAccessDerivative(AbstractFile original)
        {
            return Task.FromResult(new AccessFile(original));
        }

        private static Task<ProductionFile> GetFakeProdDerivative(AbstractFile original)
        {
            return Task.FromResult(new ProductionFile(original));
        }

        private static Task<MezzanineFile> GetFakeMezzDerivative(AbstractFile original)
        {
            return Task.FromResult(new MezzanineFile(original));
        }
    }
}