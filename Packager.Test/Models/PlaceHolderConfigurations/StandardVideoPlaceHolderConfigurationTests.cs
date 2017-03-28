using System.Collections.Generic;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Models.PlaceHolderConfigurations;

namespace Packager.Test.Models.PlaceHolderConfigurations
{
    [TestFixture]
    public class StandardVideoPlaceHolderConfigurationTests : PlaceHolderConfigurationTestsBase
    {

        [SetUp]
        public void BeforeEach()
        {
            Configuration = new StandardVideoPlaceHolderConfiguration();

            Complete = new List<AbstractFile>
            {
                MakeFile<VideoPreservationFile>(1),
                MakeFile<ProductionFile>(1),
                MakeFile<AccessFile>(1),
                MakeFile<TiffImageFile>(1)
            };

            PartiallyComplete = new List<AbstractFile>
            {
                MakeFile<VideoPreservationFile>(1),
                MakeFile<MezzanineFile>(1),
                MakeFile<AccessFile>(1),
                MakeFile<TiffImageFile>(1),
                MakeFile<VideoPreservationFile>(2),
                MakeFile<AccessFile>(2),
                MakeFile<TiffImageFile>(2)
            };

            RequiresPlaceHolders = new List<AbstractFile>
            {
                MakeFile<VideoPreservationFile>(1),
                MakeFile<MezzanineFile>(1),
                MakeFile<AccessFile>(1),
                MakeFile<TiffImageFile>(1),
                MakeFile<TiffImageFile>(2),
                MakeFile<TiffImageFile>(3),
            };

            ExpectedPlaceHolders = new List<AbstractFile>
            {
                MakeFile<VideoPreservationFile>(2),
                MakeFile<MezzanineFile>(2),
                MakeFile<AccessFile>(2),
                MakeFile<VideoPreservationFile>(3),
                MakeFile<MezzanineFile>(3),
                MakeFile<AccessFile>(3)
            };
        }

    }
}