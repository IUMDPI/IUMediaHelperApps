using NSubstitute;
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
            IUserInfoResolver userResolver = null,
            IXmlExporter xmlExporter = null)
        {
            if (bextDataProvider == null)
            {
                bextDataProvider = Substitute.For<IBextDataProvider>();
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

            var result = Substitute.For<IDependencyProvider>();

            result.BextDataProvider.Returns(bextDataProvider);
            result.CarrierDataExcelImporter.Returns(carrierDataImporter);
            result.DirectoryProvider.Returns(directoryProvider);
            result.FileProvider.Returns(fileProvider);
            result.Hasher.Returns(hasher);
            result.UserInfoResolver.Returns(userResolver);
            result.XmlExporter.Returns(xmlExporter);

            return result;
        }
    }
}