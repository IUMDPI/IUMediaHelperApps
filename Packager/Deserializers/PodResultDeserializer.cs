using System.Xml.Linq;
using Packager.Extensions;
using Packager.Factories;
using RestSharp;
using RestSharp.Deserializers;

namespace Packager.Deserializers
{
    public class PodResultDeserializer : IDeserializer
    {
        public PodResultDeserializer(IImportableFactory factory)
        {
            Factory = factory;
        }

        private IImportableFactory Factory { get; }

        public T Deserialize<T>(IRestResponse response)
        {
            if (response.Content.IsNotSet())
            {
                return default(T);
            }

            var document = XDocument.Parse(response.Content);

            return Factory.ToImportable<T>(document.Root);
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
    }
}