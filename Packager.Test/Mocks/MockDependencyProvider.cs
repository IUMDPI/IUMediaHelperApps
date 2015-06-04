using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Packager.Models;
using Packager.Models.ProcessResults;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities;

namespace Packager.Test.Mocks
{
    public static class MockDependencyProvider
    {
        public static IDependencyProvider Get(
            IBextDataProvider bextDataProvider = null,
            IExcelImporter carrierDataImporter = null,
            IDirectoryProvider directoryProvider = null,
            IFileProvider fileProvider = null,
            IHasher hasher = null,
            IProcessRunner processRunner = null,
            IProgramSettings programSettings = null,
            IUserInfoResolver userResolver = null,
            IXmlExporter xmlExporter = null,
            List<IObserver> observers = null)
        {
            if (bextDataProvider == null)
            {
                bextDataProvider = MockBextDataProvider.Get();
            }

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

            if (carrierDataImporter == null)
            {
                carrierDataImporter = Substitute.For<IExcelImporter>();
            }

            if (userResolver == null)
            {
                userResolver = Substitute.For<IUserInfoResolver>();
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
                observers = new List<IObserver>();
            }

            var result = Substitute.For<IDependencyProvider>();

            result.BextDataProvider.Returns(bextDataProvider);
            result.CarrierDataExcelImporter.Returns(carrierDataImporter);
            result.DirectoryProvider.Returns(directoryProvider);
            result.FileProvider.Returns(fileProvider);
            result.Hasher.Returns(hasher);
            result.UserInfoResolver.Returns(userResolver);
            result.XmlExporter.Returns(xmlExporter);
            result.ProcessRunner.Returns(processRunner);
            result.ProgramSettings.Returns(programSettings);
            result.Observers.Returns(observers);

            return result;
        }
    }
}