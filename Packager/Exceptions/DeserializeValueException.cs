using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packager.Exceptions
{
    public class DeserializeValueException:AbstractEngineException
    {
        public DeserializeValueException(string baseMessage, params object[] parameters) 
            : base(baseMessage, parameters)
        {
        }
    }
}
