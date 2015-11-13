using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Packager.Exceptions;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface IPodMetadataFactory<T> 
    {
        T Generate(string xml);
    }


    public abstract class AbstractPodMetadataFactory<T> : IPodMetadataFactory<T> 
    {
        public abstract T Generate(string xml);

    }

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


    public class PodVideoMetadataFactory : AbstractPodMetadataFactory<VideoPodMetadata>
    {
        public override VideoPodMetadata Generate(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                throw new ArgumentException("xml text cannot be null or empty");
            }

            var document = XDocument.Parse(xml);

            var metadata = new VideoPodMetadata();


            return metadata;

        }
    }

}
