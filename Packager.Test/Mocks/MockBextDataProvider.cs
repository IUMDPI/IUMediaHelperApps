using NSubstitute;
using Packager.Models;
using Packager.Utilities;

namespace Packager.Test.Mocks
{
    public static class MockBextDataProvider
    {
        public const string BextDescription = "Bext description";
        // ReSharper disable once InconsistentNaming
        public const string BextIARL = "Bext IARL";

        public const string BextICMT = "Bext ICMT";

        public static IBextDataProvider Get(BextData bextData = null)
        {
            if (bextData == null)
            {
                bextData = new BextData
                {
                    Description = BextDescription,
                    IARL = BextIARL,
                    ICMT = BextICMT
                };
            }

            var provider = Substitute.For<IBextDataProvider>();
            provider.GetMetadata(null).ReturnsForAnyArgs(bextData);
            return provider;
        }
    }
}