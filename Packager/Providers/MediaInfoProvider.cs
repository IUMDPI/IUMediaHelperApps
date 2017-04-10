using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.SettingsModels;
using Packager.Utilities.ProcessRunners;

namespace Packager.Providers
{
    public class MediaInfoProvider : IMediaInfoProvider
    {
        private IFFProbeRunner FFProbeRunner { get; }

        public MediaInfoProvider(IProgramSettings programSettings, IFFProbeRunner ffProbeRunner)
        {
            FFProbeRunner = ffProbeRunner;
            BaseProcessingDirectory = programSettings.ProcessingDirectory;
        }

        private string BaseProcessingDirectory { get; }

        private static int GetAudioStreamCount(XContainer element)
        {
            return element.Descendants("stream").Count(d => d.Attribute("codec_type").AttributeEquals("audio"));
        }

        private static int GetVideoStreamCount(XContainer element)
        {
            return element.Descendants("stream").Count(d => d.Attribute("codec_type").AttributeEquals("video"));
        }

        public async Task<MediaInfo> GetMediaInfo(AbstractFile target, CancellationToken cancellationToken)
        {
            var infoFile = await FFProbeRunner.GetMediaInfo(target, cancellationToken);
            var element = GetXElement(infoFile);

            return new MediaInfo
            {
                AudioStreams = GetAudioStreamCount(element),
                VideoStreams = GetVideoStreamCount(element)
            };
        }

        private XElement GetXElement(AbstractFile target)
        {
            var targetPath = Path.Combine(BaseProcessingDirectory, target.GetFolderName(), target.Filename);
            var document = XDocument.Load(targetPath);
            return document.Root;
        }

    }
}