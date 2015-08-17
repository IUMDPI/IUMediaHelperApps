using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities;
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
                ffmpegRunner.CreateDerivative(Arg.Any<ObjectFileModel>(), Arg.Any<ObjectFileModel>(), Arg.Any<string>(), Arg.Any<string>())
                    .Returns(x => Task.FromResult(x.ArgAt<ObjectFileModel>(1)));
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
            result.FFMPEGRunner.Returns(ffmpegRunner);
            return result;
        }
    }
}