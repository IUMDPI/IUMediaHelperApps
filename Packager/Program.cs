using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using Common.Models;
using Common.UserInterface.ViewModels;
using NLog.Config;
using Packager.Deserializers;
using Packager.Engine;
using Packager.Factories;
using Packager.Factories.CodingHistory;
using Packager.Factories.FFMPEGArguments;
using Packager.Models.PlaceHolderConfigurations;
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
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] arguments)
        {
            ConfigureServicePointManager();
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
                var observers = container.GetInstance<IObserverCollection>();
                observers?.LogEngineIssue(e);
            }
        }

        private static Container Bootstrap(string[] arguments)
        {
            var container = new Container();

            container.RegisterSingleton<IEngine, StandardEngine>();

            container.RegisterSingleton(() => SettingsFactory.Import(ConfigurationManager.AppSettings));
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
            container.RegisterSingleton<IFFMPEGRunner, FFMPEGRunner>();

            container.RegisterSingleton(() => new Dictionary<string, IProcessor>
            {
                {".wav", container.GetInstance<AbstractProcessor<AudioPodMetadata>>()},
                {".mkv", container.GetInstance<AbstractProcessor<VideoPodMetadata>>()}
            });

            container.RegisterSingleton<IPlaceHolderFactory, PlaceHolderFactory>();

            container.RegisterSingleton(() => new Dictionary<IMediaFormat, IPlaceHolderConfiguration>
            {
                // standard audio
                { MediaFormats.AudioCassette, new StandardAudioPlaceHolderConfiguration() },
                { MediaFormats.OpenReelAudioTape, new StandardAudioPlaceHolderConfiguration() },
                { MediaFormats.Lp, new StandardAudioPlaceHolderConfiguration() },
                { MediaFormats.Cdr, new StandardAudioPlaceHolderConfiguration() },
                { MediaFormats.FortyFive, new StandardAudioPlaceHolderConfiguration() },
                { MediaFormats.Magnabelt, new StandardAudioPlaceHolderConfiguration() },
                
                // pres-int audio
                { MediaFormats.LacquerDisc, new PresIntAudioPlaceHolderConfiguration()},
                { MediaFormats.Cylinder, new PresIntAudioPlaceHolderConfiguration()},
                { MediaFormats.SeventyEight, new PresIntAudioPlaceHolderConfiguration() },
                { MediaFormats.AluminumDisc, new PresIntAudioPlaceHolderConfiguration() },
                { MediaFormats.OtherAnalogSoundDisc, new PresIntAudioPlaceHolderConfiguration() },
                
                // standard video
                { MediaFormats.Vhs, new StandardVideoPlaceHolderConfiguration()},
                { MediaFormats.BetacamAnamorphic, new StandardVideoPlaceHolderConfiguration() },
                { MediaFormats.Dat, new StandardVideoPlaceHolderConfiguration() },
                { MediaFormats.OneInchOpenReelVideoTape, new StandardVideoPlaceHolderConfiguration() },
                { MediaFormats.HalfInchOpenReelVideoTape, new StandardVideoPlaceHolderConfiguration() },
                { MediaFormats.EightMillimeterVideo, new StandardVideoPlaceHolderConfiguration() },
                { MediaFormats.Betacam, new StandardVideoPlaceHolderConfiguration() },
                { MediaFormats.EightMillimeterVideoQuadaudio, new StandardVideoPlaceHolderConfiguration() },
                { MediaFormats.Umatic, new StandardAudioPlaceHolderConfiguration() },
                { MediaFormats.Betamax, new StandardVideoPlaceHolderConfiguration() },

            });

            container.RegisterSingleton(() => new Dictionary<IMediaFormat, ICodingHistoryGenerator>
            {
                { MediaFormats.OpenReelAudioTape, new StandardCodingHistoryGenerator() },
                { MediaFormats.AudioCassette, new StandardCodingHistoryGenerator()},
                { MediaFormats.Cdr, new CdrCodingHistoryGenerator() },
                { MediaFormats.LacquerDisc, new LacquerOrCylinderCodingHistoryGenerator()},
                { MediaFormats.LacquerDiscIrene, new LacquerDiscIreneCodingHistoryGenerator()},
                { MediaFormats.Cylinder, new LacquerOrCylinderCodingHistoryGenerator() },
                { MediaFormats.AluminumDisc, new LacquerOrCylinderCodingHistoryGenerator()},
                { MediaFormats.OtherAnalogSoundDisc, new LacquerOrCylinderCodingHistoryGenerator() },
                { MediaFormats.SeventyEight, new SeventyEightCodingHistoryGenerator() },
                { MediaFormats.Magnabelt, new MagnabeltCodingHistoryGenerator() }
            });

            container.RegisterSingleton(() => new Dictionary<IMediaFormat, IFFMPEGArgumentsGenerator>
                {
                    {MediaFormats.AluminumDisc, new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.AudioCassette, new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.Betacam, new VideoFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.BetacamAnamorphic, new VideoFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.Betamax, new VideoFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.Cdr, new CdrFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.Cylinder,new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.Dat, new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.EightMillimeterVideo, new VideoFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.EightMillimeterVideoQuadaudio, new VideoFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.FortyFive, new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.LacquerDisc, new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.LacquerDiscIrene, new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.Lp, new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.Magnabelt, new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.OneInchOpenReelVideoTape, new VideoFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.HalfInchOpenReelVideoTape, new VideoFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.OpenReelAudioTape,new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.OtherAnalogSoundDisc, new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.SeventyEight, new AudioFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.Umatic, new VideoFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())},
                    {MediaFormats.Vhs, new VideoFFMPEGArgumentsGenerator(container.GetInstance<IProgramSettings>())}
            });

            container.RegisterSingleton<IFFMPEGArgumentsFactory, FFMPEGArgumentsFactory>();

            container.RegisterSingleton(() => new CancellationTokenSource());

            container.RegisterSingleton(() => GetRestClient(
                container.GetInstance<IProgramSettings>(),
                container.GetInstance<IFileProvider>(),
                container.GetInstance<IImportableFactory>()));

            container.RegisterSingleton<IValidatorCollection>(() => new StandardValidatorCollection
            {
                new ValueRequiredValidator(),
                new DirectoryExistsValidator(container.GetInstance<IDirectoryProvider>()),
                new FileExistsValidator(container.GetInstance<IFileProvider>()),
                new UriValidator(),
                new MembersValidator()
            });

            container.RegisterSingleton<IObserverCollection>(() => new ObserverCollection
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
            var podAuth = fileProvider.Deserialize<PodAuth>(programSettings.PodAuthFilePath) ??
                          new PodAuth();
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

        private static void ConfigureServicePointManager()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
    }
}
