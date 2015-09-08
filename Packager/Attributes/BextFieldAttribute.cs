using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Utilities;

namespace Packager.Attributes
{
    public class BextFieldAttribute:Attribute
    {
        public BextFields Field { get; set; }

        public BextFieldAttribute(BextFields field)
        {
            Field = field;
        }
    }
}
