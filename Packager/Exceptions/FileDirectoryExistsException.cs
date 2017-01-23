using System;
using Common.Exceptions;

namespace Packager.Exceptions
{
    public class FileDirectoryExistsException:AbstractEngineException
    {
        public FileDirectoryExistsException(string baseMessage, params object[] parameters) : base(baseMessage, parameters)
        {
        }

        public FileDirectoryExistsException(Exception innerException, string baseMessage, params object[] parameters) : base(innerException, baseMessage, parameters)
        {
        }
    }
}
