using Packager.Models;
using Packager.Observers;
using Packager.Utilities;

namespace Packager.Providers
{
    public class DefaultDependencyProvider : IDependencyProvider
    {
        public DefaultDependencyProvider(IProgramSettings programSettings, IObserverCollection observers)
        {
            Hasher = new Hasher();
            UserInfoResolver = new DomainUserResolver();
            XmlExporter = new XmlExporter();
            DirectoryProvider = new DirectoryProvider();
            FileProvider = new FileProvider();
            ProcessRunner = new ProcessRunner();
            ProgramSettings = programSettings;
            MetadataGenerator = new MetadataGenerator(Hasher);
            Observers = observers;
            LookupsProvider = new AppConfigLookupsProvider();
            MetadataProvider = new PodMetadataProvider(ProgramSettings, LookupsProvider);
            BextProcessor = new BextProcessor(programSettings, FileProvider, ProcessRunner, XmlExporter, observers);
        }

        public IHasher Hasher { get; private set; }
        public IUserInfoResolver UserInfoResolver { get; private set; }
        public IXmlExporter XmlExporter { get; private set; }
        public IDirectoryProvider DirectoryProvider { get; private set; }
        public IFileProvider FileProvider { get; private set; }
        public IProcessRunner ProcessRunner { get; private set; }
        public IMetadataGenerator MetadataGenerator { get; private set; }
        public IProgramSettings ProgramSettings { get; private set; }
        public IObserverCollection Observers { get; private set; }
        public IPodMetadataProvider MetadataProvider { get; private set; }
        public ILookupsProvider LookupsProvider { get; private set; }
        public IBextProcessor BextProcessor { get; private set; }
    }
}