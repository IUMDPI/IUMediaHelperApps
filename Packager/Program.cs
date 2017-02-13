using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Common.UserInterface.ViewModels;
using NLog.Config;
using Packager.Deserializers;
using Packager.Engine;
using Packager.Factories;
using Packager.Models.PodMetadataModels;
using Packager.Models.ProgramArgumentsModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Observers.LayoutRenderers;
using Packager.Processors;
using Packager.Providers;
using Packager.UserInterface;
using Packager.Utilities.Bext;
using Packager.Utilities.Configuration;
using Packager.Utilities.Email;
using Packager.Utilities.FileSystem;
using Packager.Utilities.Hashing;
using Packager.Utilities.Images;
using Packager.Utilities.ProcessRunners;
using Packager.Utilities.Reporting;
using Packager.Utilities.Xml;
using Packager.Validators;
using Packager.Verifiers;
using RestSharp;
using RestSharp.Authenticators;
using SimpleInjector;

namespace Packager
{
    static class Program
    {
        [STAThread]
        static void Main(string[] arguments)
        {
            ConfigureNLog();

            var container = Bootstrap(arguments);
            RunApplication(container);
        }

        private static void RunApplication(Container container)
        {
            try
            {
                var outputWindow = container.GetInstance<OutputWindow>();

                var app = new App();
                app.Run(outputWindow);
            }
            catch (Exception e)
            {
               // todo: log
            }
        }

        private static Container Bootstrap(string[] arguments)
        {
            var container = new Container();
            
            container.RegisterSingleton<IEngine, StandardEngine>();

            container.RegisterSingleton (()=>SettingsFactory.Import(ConfigurationManager.AppSettings));
            container.RegisterSingleton<IProgramArguments>(new ProgramArguments(arguments));
            container.RegisterSingleton<IHasher, Hasher>();
            container.RegisterSingleton<IXmlExporter, XmlExporter>();
            container.RegisterSingleton<IDirectoryProvider, DirectoryProvider>();
            container.RegisterSingleton<IProcessRunner, ProcessRunner>();
            container.RegisterSingleton<IFileProvider, FileProvider>();
            container.RegisterSingleton<IIngestDataFactory, IngestDataFactory>();
            container.RegisterSingleton<ISideDataFactory, SideDataFactory>();
            container.RegisterSingleton<ICarrierDataFactory<AudioPodMetadata>, AudioCarrierDataFactory>();
            container.RegisterSingleton<ICarrierDataFactory<VideoPodMetadata>, VideoCarrierDataFactory>();
            container.RegisterSingleton<ISystemInfoProvider, SystemInfoProvider>();
            container.RegisterSingleton<IEmbeddedMetadataFactory<AudioPodMetadata>, EmbeddedAudioMetadataFactory>();
            container.RegisterSingleton<IEmbeddedMetadataFactory<VideoPodMetadata>, EmbeddedVideoMetadataFactory>();
            container.RegisterSingleton<IBwfMetaEditRunner, BwfMetaEditRunner>();
            container.RegisterSingleton<IBwfMetaEditResultsVerifier, BwfMetaEditResultsVerifier>();
            container.RegisterSingleton<IBextProcessor, BextProcessor>();
            container.RegisterSingleton<IEmailSender, EmailSender>();
            container.RegisterSingleton<IImportableFactory, ImportableFactory>();
            container.RegisterSingleton<IPodMetadataProvider, PodMetadataProvider>();
            container.RegisterSingleton<ISuccessFolderCleaner, SuccessFolderCleaner>();
            container.RegisterSingleton<IFFProbeRunner, FFProbeRunner>();
            container.RegisterSingleton<IMediaInfoProvider, MediaInfoProvider>();
            container.RegisterSingleton<IConfigurationLogger, ConfigurationLogger>();
            container.RegisterSingleton<ILabelImageImporter, LabelImageImporter>();
            container.RegisterSingleton<AbstractProcessor<AudioPodMetadata>, AudioProcessor>();
            container.RegisterSingleton<AbstractProcessor<VideoPodMetadata>, VideoProcessor>();

            container.RegisterConditional<IFFMPEGRunner, VideoFFMPEGRunner>(Lifestyle.Singleton, 
                c=>c.Consumer.ImplementationType == typeof(VideoProcessor));

            container.RegisterConditional<IFFMPEGRunner, AudioFFMPEGRunner>(Lifestyle.Singleton,
                c => c.Consumer.ImplementationType == typeof(AudioProcessor) || c.Consumer.ImplementationType == typeof(ConfigurationLogger));

            container.RegisterSingleton( ()=> new Dictionary<string, IProcessor>
            {
                {".wav", container.GetInstance<AbstractProcessor<AudioPodMetadata>>()},
                {".mkv", container.GetInstance<AbstractProcessor<VideoPodMetadata>>()}
            });

            container.RegisterSingleton(()=>new CancellationTokenSource());

            container.RegisterSingleton(()=>GetRestClient(
                container.GetInstance<IProgramSettings>(), 
                container.GetInstance<IFileProvider>(), 
                container.GetInstance<IImportableFactory>())) ;

            container.RegisterSingleton<IValidatorCollection>(()=> new StandardValidatorCollection
            {
                new ValueRequiredValidator(),
                new DirectoryExistsValidator(container.GetInstance<IDirectoryProvider>()),
                new FileExistsValidator(container.GetInstance<IFileProvider>()),
                new UriValidator(),
                new MembersValidator()
            });
            
            container.RegisterSingleton<IObserverCollection>(()=>new ObserverCollection()
                {
                    new GeneralNLogObserver(container.GetInstance<IProgramSettings>()),
                    new ObjectNLogObserver(container.GetInstance<IProgramSettings>()),
                    new ViewModelObserver(container.GetInstance<ILogPanelViewModel>()),
                    new IssueEmailerObserver(
                        container.GetInstance<ProgramSettings>(), 
                        container.GetInstance<ISystemInfoProvider>(), 
                        container.GetInstance<IEmailSender>())
                });

            container.RegisterSingleton<IViewModel, ViewModel>();
            container.RegisterSingleton<ILogPanelViewModel, LogPanelViewModel>();
            container.RegisterSingleton<OutputWindow>();
            container.RegisterSingleton<IReportWriter, ReportWriter>();

            container.Verify();

            return container;
            
        }

        private static IRestClient GetRestClient(IProgramSettings programSettings, IFileProvider fileProvider,
            IImportableFactory factory)
        {
            var podAuth = fileProvider.Deserialize<PodAuth>(programSettings.PodAuthFilePath);
            var result = new RestClient(programSettings.WebServiceUrl)
            {
                Authenticator = new HttpBasicAuthenticator(podAuth.UserName, podAuth.Password)
            };

            result.AddHandler("application/xml", new PodResultDeserializer(factory));
            return result;
        }

        private static void ConfigureNLog()
        {
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("ProjectCode",
                typeof(ProjectCodeLayoutRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("LogDirectoryName",
                typeof(LoggingDirectoryLayoutRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("Barcode",
                typeof(BarcodeLayoutRenderer));

        }
    }
}
