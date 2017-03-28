using System.Collections.Generic;
using Common.Models;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Models.PlaceHolderConfigurations;

namespace Packager.Test.Models.PlaceHolderConfigurations
{
    [TestFixture]
    public class DynamicAudioPlaceHolderConfigurationTests : PlaceHolderConfigurationTestsBase
    {
        [SetUp]
        public void BeforeEach()
        {
            Configuration = new DynamicAudioPlaceholderConfiguration(
                new List<FileUsages>
                {
                    FileUsages.PreservationMaster,
                    FileUsages.ProductionMaster,
                    FileUsages.AccessFile
                });

            Complete = new List<AbstractFile>
            {
                MakeFile<AudioPreservationFile>(1),
                MakeFile<ProductionFile>(1),
                MakeFile<AccessFile>(1),
                MakeFile<TiffImageFile>(1)
            };

            PartiallyComplete = new List<AbstractFile>
            {
                MakeFile<AudioPreservationFile>(1),
                MakeFile<ProductionFile>(1),
                MakeFile<AccessFile>(1),
                MakeFile<TiffImageFile>(1),
                MakeFile<AudioPreservationFile>(2),
                MakeFile<AccessFile>(2),
                MakeFile<TiffImageFile>(2)
            };

            RequiresPlaceHolders = new List<AbstractFile>
            {
                MakeFile<AudioPreservationFile>(1),
                MakeFile<ProductionFile>(1),
                MakeFile<AccessFile>(1),
                MakeFile<TiffImageFile>(1),
                MakeFile<TiffImageFile>(2),
                MakeFile<TiffImageFile>(3),
            };

            ExpectedPlaceHolders = new List<AbstractFile>
            {
                MakeFile<AudioPreservationFile>(2),
                MakeFile<ProductionFile>(2),
                MakeFile<AccessFile>(2),
                MakeFile<AudioPreservationFile>(3),
                MakeFile<ProductionFile>(3),
                MakeFile<AccessFile>(3)
            };
        }

    }

    [TestFixture]
    public class DynamicVideoPlaceHolderConfigurationTests : PlaceHolderConfigurationTestsBase
    {
        [SetUp]
        public void BeforeEach()
        {
            Configuration = new DynamicVideoPlaceholderConfiguration(
                new List<FileUsages>
                {
                    FileUsages.PreservationMaster,
                    FileUsages.MezzanineFile,
                    FileUsages.AccessFile
                });

            Complete = new List<AbstractFile>
            {
                MakeFile<VideoPreservationFile>(1),
                MakeFile<MezzanineFile>(1),
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
