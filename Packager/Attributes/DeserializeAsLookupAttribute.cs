using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Providers;
using RestSharp.Deserializers;

namespace Packager.Attributes
{
    public class DeserializeAsLookupAttribute:Attribute
    {
        public string Name { get; set; }
        public LookupTables LookupTable { get; set; }
        
        public DeserializeAsLookupAttribute()
        {
            
        }
    }
}
