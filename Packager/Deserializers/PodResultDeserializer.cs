using System;
using System.Xml.Linq;
using Packager.Extensions;
using RestSharp;
using RestSharp.Deserializers;

namespace Packager.Deserializers
{
    public interface IImportableFromPod
    {
        void ImportFromXml(XElement element);
    }

    public class PodResultDeserializer : IDeserializer
    {
        public T Deserialize<T>(IRestResponse response)
        {
            if (string.IsNullOrEmpty(response.Content))
            {
                return default(T);
            }

            var document = XDocument.Parse(response.Content);

            return document.Root.ToImportable<T>();
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
    }
}