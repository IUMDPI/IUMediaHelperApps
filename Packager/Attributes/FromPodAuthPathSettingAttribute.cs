using System;
using System.IO;
using System.Xml.Serialization;
using Packager.Models;

namespace Packager.Attributes
{
    public class FromPodAuthPathSettingAttribute : AbstractFromConfigSettingAttribute
    {
        public FromPodAuthPathSettingAttribute(string name, bool required = true)
            : base(name, required)
        {
        }

        public override object Convert(string value)
        {
            try
            {
                var serializer = new XmlSerializer(typeof (PodAuth));
                using (var stream = new FileStream(value, FileMode.Open))
                {
                    return (PodAuth) serializer.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Issues.Add(e.Message);
                return null;
            }
            
        }
    }
}