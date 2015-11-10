using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Packager.Models.PodMetadataModels.ConsolidatedModels;
using RestSharp;
using RestSharp.Deserializers;

namespace Packager.Deserializers
{
    public interface IImportableFromPod
    {
        void ImportFromXml(XDocument document);
        void ImportFromXml(XElement element);
    }

    public class PodResultDeserializer:IDeserializer
    {
        public T Deserialize<T>(IRestResponse response)
        {
            if (string.IsNullOrEmpty(response.Content))
            {
                return default(T);
            }

            var document = XDocument.Parse(response.Content);

            var result = Activator.CreateInstance<T>();
            var typedResult = result as AbstractConsolidatedPodMetadata;
            if (typedResult == null)
            {
                throw new InvalidCastException($"cannot cast {typeof(T).Name} to AbstractConsolidatedPodMetadata");
            }

            typedResult.ImportFromXml(document);

            return result;
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
    }
}
