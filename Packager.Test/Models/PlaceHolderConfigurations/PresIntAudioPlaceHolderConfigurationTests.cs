using System.Collections.Generic;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Models.PlaceHolderConfigurations;

namespace Packager.Test.Models.PlaceHolderConfigurations
{
    [TestFixture]
    public class PresIntAudioPlaceHolderConfigurationTests : PlaceHolderConfigurationTestsBase
    {

        [SetUp]
        public void BeforeEach()
        {
            Configuration = new PresIntAudioPlaceHolderConfiguration();

            Complete = new List<AbstractFile>
            {
                MakeFile<AudioPreservationFile>(1),
                MakeFile<AudioPreservationIntermediateFile>(1),
                MakeFile<ProductionFile>(1),
                MakeFile<AccessFile>(1),
                MakeFile<TiffImageFile>(1)
            };

            PartiallyComplete = new List<AbstractFile>
            {
                MakeFile<AudioPreservationFile>(1),
                MakeFile<AudioPreservationIntermediateFile>(1),
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
                MakeFile<AudioPreservationIntermediateFile>(1),
                MakeFile<ProductionFile>(1),
                MakeFile<AccessFile>(1),
                MakeFile<TiffImageFile>(1),
                MakeFile<TiffImageFile>(2),
                MakeFile<TiffImageFile>(3),
            };

            ExpectedPlaceHolders = new List<AbstractFile>
            {
                MakeFile<AudioPreservationFile>(2),
                MakeFile<AudioPreservationIntermediateFile>(2),
                MakeFile<ProductionFile>(2),
                MakeFile<AccessFile>(2),
                MakeFile<AudioPreservationFile>(3),
                MakeFile<AudioPreservationIntermediateFile>(3),
                MakeFile<ProductionFile>(3),
                MakeFile<AccessFile>(3)
            };
        }

    }
}