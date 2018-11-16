using Common.Exceptions;
using Common.Models;

namespace Packager.Exceptions
{
    public class FormatNotSupportedException: AbstractEngineException
    {
        public FormatNotSupportedException(IMediaFormat format) 
            : base($"The format {format.ProperName} ({format.Key}) is not currently supported")
        {
        }
    }
}
