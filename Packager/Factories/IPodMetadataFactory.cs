using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Exceptions;

namespace Packager.Factories
{
    public interface IPodMetadataFactory<T> 
    {
        T Generate(string xml);
    }
}
