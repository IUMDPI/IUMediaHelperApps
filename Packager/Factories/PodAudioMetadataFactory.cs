using System;
using System.Xml.Linq;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class PodAudioMetadataFactory : AbstractPodMetadataFactory<AudioPodMetadata>
    {
        public override AudioPodMetadata Generate(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                throw new ArgumentException("xml text cannot be null or empty");
            }

            var document = XDocument.Parse(xml);

            var metadata = new AudioPodMetadata();


            return metadata;

        }
    }
}