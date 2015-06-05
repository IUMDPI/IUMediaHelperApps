using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.Test.Mocks;

namespace Packager.Test.Processors
{
    [TestFixture]
    public class AudioProcessorTests
    {
        protected const string ProjectCode = "mdpi";
        protected const string BarCode1 = "4890764553278906";
        protected const string BarCode2 = "7890764553278907";

        private const string ExcelFileName = "MDPI_4890764553278906.xlxs";
        private const string PreservationFileName1 = "MDPI_4890764553278906_01_pres.wav";
        private const string PreservationIntFileName = "MDPI_4890764553278906_01_pres-int.wav";
        private const string PreservationFileName2 = "MDPI_4890764553278906_02_pres.wav";

        private const string InvalidFileName = "MDPI_4890764553278906_pres.wav";

        private readonly ExcelFileModel _excelModel = new ExcelFileModel(ExcelFileName);
        private readonly ObjectFileModel _wavModel1 = new ObjectFileModel(PreservationFileName1);
        private readonly ObjectFileModel _wavModel2 = new ObjectFileModel(PreservationFileName2);
        private readonly ObjectFileModel _invalidFileModel = new ObjectFileModel(InvalidFileName);

        private IGrouping<string, AbstractFileModel> GetGrouping()
        {
            var list = new List<AbstractFileModel> { _excelModel, _wavModel1, _wavModel2 };
            return list.GroupBy(m => m.BarCode).First();
        }

        private static AudioProcessor GetProcessor(
            IProgramSettings settings = null,
            IDependencyProvider dependencyProvider = null,
            List<IObserver> observers = null,
            IFileProvider fileProvider = null)
        {
            if (dependencyProvider == null)
            {
                dependencyProvider =
                    MockDependencyProvider.Get(
                        programSettings: settings,
                        observers: observers,
                        fileProvider: fileProvider);
            }

            return new AudioProcessor(dependencyProvider);
        }

        [TestFixture]
        public class WhenProcessorStarts : AudioProcessorTests
        {
            [Test]
            public async void ItShouldCreateProcessingDirectoryWithUpperCaseProjectName()
            {
                var directoryProvider = Substitute.For<IDirectoryProvider>();
                var dependencyProvider = MockDependencyProvider.Get(directoryProvider: directoryProvider);

                var processor = GetProcessor(dependencyProvider: dependencyProvider);

                await processor.ProcessFile(GetGrouping());

                var expectedDirectory = Path.Combine(
                    MockProgramSettings.ProcessingDirectory,
                    string.Format("{0}_{1}", ProjectCode.ToUpperInvariant(), BarCode1));

                directoryProvider.Received().CreateDirectory(Arg.Is(expectedDirectory));
            }

            [Test]
            public async void ItShouldLogBatchProcessingHeader()
            {
                var mockObserver = Substitute.For<IObserver>();
                var processor = GetProcessor(observers: new List<IObserver> { mockObserver });

                await processor.ProcessFile(GetGrouping());

                mockObserver.Received().LogHeader(Arg.Is("Processing object {0}"), Arg.Is(BarCode1));
            }

            [Test]
            public async void ItShouldLogExcelSpreadsheet()
            {
                var mockObserver = Substitute.For<IObserver>();
                var processor = GetProcessor(observers: new List<IObserver> { mockObserver });

                await processor.ProcessFile(GetGrouping());

                mockObserver.Received().Log(Arg.Is("Spreadsheet: {0}"), Arg.Is(ExcelFileName));
            }

            [Test]
            public async void ItShouldLogFilesMovedToProcessingDirectory()
            {
                var mockObserver = Substitute.For<IObserver>();
                var processor = GetProcessor(observers: new List<IObserver> { mockObserver });

                await processor.ProcessFile(GetGrouping());

                mockObserver.Received().Log(Arg.Is("Moving file to processing: {0}"), Arg.Is(PreservationFileName1));
                mockObserver.Received().Log(Arg.Is("Moving file to processing: {0}"), Arg.Is(PreservationFileName2));
            }

            [Test]
            public void ItShouldThrowExceptionIfNoSpreadSheetModelPresent()
            {
                var processor = GetProcessor();
                var grouping = new List<AbstractFileModel> { _wavModel1, _wavModel2 }
                    .GroupBy(m => m.BarCode).First();

                var exception = Assert.Throws<Exception>(async () => await processor.ProcessFile(grouping));
                Assert.That(exception.Message, Is.EqualTo("No input data spreadsheet in batch"));

            }

            [Test]
            public async void ItShouldMoveExcelSpreadsheetToProcessingDirectory()
            {
                var fileProvider = Substitute.For<IFileProvider>();
                var processor = GetProcessor(fileProvider: fileProvider);

                await processor.ProcessFile(GetGrouping());

                fileProvider.Received().Move(
                    Arg.Is<string>(s => s.EndsWith(ExcelFileName)), 
                    Arg.Is<string>(s => s.EndsWith(ExcelFileName)));

            }
        }









    }
}