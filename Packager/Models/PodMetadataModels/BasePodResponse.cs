using System.Xml.Linq;
using Packager.Deserializers;
using Packager.Extensions;
using Packager.Providers;

namespace Packager.Models.PodMetadataModels
{
    public class BasePodResponse : IImportable
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public virtual void ImportFromXml(XElement element, ILookupsProvider lookupsProvider)
        {
            Success = element.ToBooleanValue("success");
            Message = element.ToStringValue("message");
        }
    }
}