using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.Test.Mocks;
using Packager.Utilities;

namespace Packager.Test.Processors
{
    
    public abstract class AbstractProcessorTests
    {
        protected IProgramSettings ProgramSettings { get; set; }
        protected IDependencyProvider DependencyProvider { get; set; }
        protected IDirectoryProvider DirectoryProvider { get; set; }
        protected IFileProvider FileProvider { get; set; }
        protected IHasher Hasher { get; set; }
        protected IProcessRunner ProcessRunner { get; set; }
        protected IUserInfoResolver UserInfoResolver { get; set; }
        protected IXmlExporter XmlExporter { get; set; }
        protected IObserver Observer { get; set; }
        protected IProcessor Processor { get; set; }

        protected ObjectFileModel PresObjectFileModel { get; set; }
        protected ObjectFileModel ProdObjectFileModel { get; set; }

        private IGrouping<string, AbstractFileModel> GetGrouping()
        {
            var list = new List<AbstractFileModel> {PresObjectFileModel, ProdObjectFileModel };
            return list.GroupBy(m => m.BarCode).First();
        }

        [SetUp]
        public virtual void BeforeEach()
        {
            ProgramSettings = Substitute.For<IProgramSettings>();
            DirectoryProvider = Substitute.For<IDirectoryProvider>();
            FileProvider = Substitute.For<IFileProvider>();
            Hasher = Substitute.For<IHasher>();
            ProcessRunner = Substitute.For<IProcessRunner>();
            UserInfoResolver = Substitute.For<IUserInfoResolver>();
            XmlExporter = Substitute.For<IXmlExporter>();
            Observer = Substitute.For<IObserver>();
            


            DependencyProvider = MockDependencyProvider.Get(DirectoryProvider, FileProvider, Hasher, ProcessRunner, ProgramSettings, UserInfoResolver, XmlExporter, new List<IObserver>() {Observer});
        }

        public class WhenProcessingFiles : AbstractProcessorTests
        {
            protected const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";
            protected const string ProductionFileName = "MDPI_4890764553278906_01_pres.wav";

            public async override void BeforeEach()
            {
                base.BeforeEach();

                PresObjectFileModel = new ObjectFileModel(PreservationFileName);
                ProdObjectFileModel = new ObjectFileModel(ProductionFileName);

                Processor = new AudioProcessor(DependencyProvider);
                await Processor.ProcessFile(GetGrouping());
            }

            [Test]
            public void ItShouldCallBeginSectionCorrectly()
            {
               
            }
        }


    }
}
