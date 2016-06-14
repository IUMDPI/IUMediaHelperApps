using System;
using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities;
using Packager.Utilities.FileSystem;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class SuccessFolderCleanerTests
    {
        [SetUp]
        public virtual void BeforeEach()
        {
            Observers = Substitute.For<IObserverCollection>();
            Interval = 5;
            DirectoryProvider = Substitute.For<IDirectoryProvider>();

            DoCustomSetup();

            var programSettings = Substitute.For<IProgramSettings>();
            programSettings.SuccessDirectoryName.Returns(SuccessFolder);
            programSettings.DeleteSuccessfulObjectsAfterDays.Returns(Interval);
            Cleaner = new SuccessFolderCleaner(programSettings, DirectoryProvider, Observers);
        }

        private const string SuccessFolder = "success folder";
        private IObserverCollection Observers { get; set; }
        private IDirectoryProvider DirectoryProvider { get; set; }

        private int Interval { get; set; }

        private SuccessFolderCleaner Cleaner { get; set; }

        protected virtual void DoCustomSetup()
        {
        }

        public class WhenThereIsNothingToClean : SuccessFolderCleanerTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();
                await Cleaner.DoCleaning();
            }

            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();

                DirectoryProvider.GetFolderNamesAndCreationDates(SuccessFolder).Returns(new List<KeyValuePair<string, DateTime>>());
            }

            [Test]
            public void ItShouldNotAttemptToRemoveFolders()
            {
                DirectoryProvider.DidNotReceive().DeleteDirectoryAsync(Arg.Any<string>());
            }

            [Test]
            public void ItShouldNotCloseSection()
            {
                Observers.DidNotReceive().EndSection(Arg.Any<string>());
            }

            [Test]
            public void ItShouldNotOpenSection()
            {
                Observers.DidNotReceive().BeginSection("Removing {0} from success folder", Arg.Any<string>());
            }
        }

        public class WhenIntervalIsNotLessThanOneDay : SuccessFolderCleanerTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                Interval = 0;
            }

            [Test]
            public void EnabledShouldReturnFalse()
            {
                Assert.That(Cleaner.Enabled, Is.False);
            }
        }

        public class WhenIntervalIsOneDayOrMore : SuccessFolderCleanerTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                Interval = 1;
            }

            [Test]
            public void EnabledShouldReturnTrue()
            {
                Assert.That(Cleaner.Enabled, Is.True);
            }
        }

        public class WhenThereAreObjectsToClean : SuccessFolderCleanerTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();
                await Cleaner.DoCleaning();
            }

            private KeyValuePair<string, DateTime> Folder1 { get; set; }
            private KeyValuePair<string, DateTime> Folder2 { get; set; }
            private KeyValuePair<string, DateTime> Folder3 { get; set; }

            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();

                Folder1 = new KeyValuePair<string, DateTime>("Folder1", DateTime.Now.Subtract(new TimeSpan(6, 0, 0, 0))); // six days old

                Folder2 = new KeyValuePair<string, DateTime>("Folder2", DateTime.Now.Subtract(new TimeSpan(3, 0, 0, 0))); // 3 days old

                Folder3 = new KeyValuePair<string, DateTime>("Folder3", DateTime.Now.Subtract(new TimeSpan(6, 0, 0, 0))); // 8 days old

                DirectoryProvider.GetFolderNamesAndCreationDates(SuccessFolder).Returns(new List<KeyValuePair<string, DateTime>> {Folder1, Folder2, Folder3});
            }


            [TestCase("Folder1", true)]
            [TestCase("Folder2", false)]
            [TestCase("Folder3", true)]
            public void ItShouldCallDeleteDirectoryCorrectly(string folderName, bool expectedCall)
            {
                if (expectedCall)
                {
                    DirectoryProvider.Received().DeleteDirectoryAsync(Path.Combine(SuccessFolder, folderName));
                }
                else
                {
                    DirectoryProvider.DidNotReceive().DeleteDirectoryAsync(Path.Combine(SuccessFolder, folderName));
                }
            }

            [TestCase("Folder1", true)]
            [TestCase("Folder2", false)]
            [TestCase("Folder3", true)]
            public void ItLogDeletedDirectoriesCorrectly(string folderName, bool expectedCall)
            {
                if (expectedCall)
                {
                    Observers.Received().Log("{0} removed", folderName);
                }
                else
                {
                    Observers.DidNotReceive().Log("{0} removed", folderName);
                }
            }

            [Test]
            public void ItShouldCloseSection()
            {
                Observers.Received().EndSection(Arg.Any<string>());
            }

            [Test]
            public void ItShouldOpenSection()
            {
                Observers.Received().BeginSection("Removing {0} from success folder", Arg.Any<string>());
            }
        }
    }
}