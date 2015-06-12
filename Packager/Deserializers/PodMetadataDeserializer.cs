using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using Packager.Attributes;
using Packager.Extensions;
using Packager.Providers;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Extensions;

namespace Packager.Deserializers
{
    public class PodMetadataDeserializer : IDeserializer
    {
        private const string CallNumberElementName = "call-number";
        private const string BarcodeElementName = "mdpi-barcode";
        private const string TitleElementName = "title";
        private readonly ILookupsProvider _lookupsProvider;

        public PodMetadataDeserializer(ILookupsProvider lookupsProvider)
        {
            _lookupsProvider = lookupsProvider;
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        public virtual T Deserialize<T>(IRestResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }

            var xml = XElement.Parse(response.Content);

            var target = Activator.CreateInstance<T>();
            SetValues(target, xml);

            return target;
        }

        private void SetValues<T>(T target, XElement xml)
        {
            SetStandardProperties(target, xml);
            SetLookupProperties(target, xml, _lookupsProvider);
        }

        private static void SetStandardProperties<T>(T target, XElement xml)
        {
            foreach (var property in GetStandardProperties(target.GetType()))
            {
                var attribute = property.GetCustomAttribute<DeserializeAsAttribute>();
                var stringValue = xml.GetValueFromDescendent(attribute.Name);
                var value = ConvertToCorrectType(stringValue, property);
                if (value == null)
                {
                    throw new Exception(string.Format("Could not convert value of {0} for field {1} to a known type", stringValue, property.Name));
                }

                property.SetValue(target, value);
            }
        }

        private static void SetLookupProperties<T>(T target, XElement xml, ILookupsProvider lookups)
        {
            foreach (var property in GetLookupProperties(target.GetType()))
            {
                var attribute = property.GetCustomAttribute<DeserializeAsLookupAttribute>();
                object value;
                if (property.PropertyType == typeof (string))
                {
                    var originalValue = xml.GetValueFromDescendent(attribute.Name);
                    value = lookups.LookupValue(attribute.LookupTable, originalValue);
                }
                else if (property.PropertyType == typeof (string[]))
                {
                    var originalValues = xml.GetGroupValuesFromDescendent(attribute.Name);
                    value = lookups.LookupValue(attribute.LookupTable, originalValues);
                }
                else
                {
                    throw new Exception("Property value must be string or string array");
                }

                property.SetValue(target, value);
            }
        }

        private static IEnumerable<PropertyInfo> GetStandardProperties(Type type)
        {
            return type.GetProperties().Where(HasDeserializeAttribute);
        }

        private static IEnumerable<PropertyInfo> GetLookupProperties(Type type)
        {
            return type.GetProperties().Where(HasDeserializeAsLookupAttribute);
        }

        private static bool HasDeserializeAttribute(PropertyInfo info)
        {
            return info.GetCustomAttribute<DeserializeAsAttribute>() != null;
        }

        private static bool HasDeserializeAsLookupAttribute(PropertyInfo info)
        {
            return info.GetCustomAttribute<DeserializeAsLookupAttribute>() != null;
        }

        private static object ConvertToCorrectType(string value, PropertyInfo info)
        {
            if (info.PropertyType == typeof (string))
            {
                return value;
            }

            if (info.PropertyType.IsPrimitive)
            {
                return value.ChangeType(info.PropertyType);
            }

            return null;
        }
    }
}