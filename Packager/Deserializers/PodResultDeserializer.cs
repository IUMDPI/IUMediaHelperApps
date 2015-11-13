using System;
using System.Xml.Linq;
using Packager.Extensions;
using Packager.Providers;
using RestSharp;
using RestSharp.Deserializers;

namespace Packager.Deserializers
{
    public class PodResultDeserializer : IDeserializer
    {
        private ILookupsProvider LookupsProvider { get; }

        public PodResultDeserializer(ILookupsProvider lookupsProvider)
        {
            LookupsProvider = lookupsProvider;
        }

        public T Deserialize<T>(IRestResponse response)
        {
            if (string.IsNullOrEmpty(response.Content))
            {
                return default(T);
            }

            var document = XDocument.Parse(response.Content);

            return document.Root.ToImportable<T>(LookupsProvider);
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
    }
}