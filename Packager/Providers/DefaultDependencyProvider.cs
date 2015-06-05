using System.Collections.Generic;
using Packager.Models;
using Packager.Observers;
using Packager.Utilities;

namespace Packager.Providers
{
    public class DefaultDependencyProvider : IDependencyProvider
    {
        public DefaultDependencyProvider(IProgramSettings programSettings, List<IObserver> observers)
        {
            BextDataProvider = new StandInBextDataProvider();
            Hasher = new Hasher();
            UserInfoResolver = new DomainUserResolver();
            XmlExporter = new XmlExporter();
            DirectoryProvider = new DirectoryProvider();
            FileProvider = new FileProvider();
            ProcessRunner = new ProcessRunner();
            ProgramSettings = programSettings;
            MetadataGenerator = new MetadataGenerator(FileProvider, Hasher, ProgramSettings, UserInfoResolver);
            Observers = observers;
            MetadataProvider = new PodMetadataProvider(ProgramSettings);
        }

        public IBextDataProvider BextDataProvider { get; private set; }
        public IHasher Hasher { get; private set; }
        public IUserInfoResolver UserInfoResolver { get; private set; }
        public IXmlExporter XmlExporter { get; private set; }
        public IDirectoryProvider DirectoryProvider { get; private set; }
        public IFileProvider FileProvider { get; private set; }
        public IProcessRunner ProcessRunner { get; private set; }
        public IMetadataGenerator MetadataGenerator { get; private set; }
        public IProgramSettings ProgramSettings { get; private set; }
        public List<IObserver> Observers { get; private set; }
        public IPodMetadataProvider MetadataProvider { get; private set; }
    }
}