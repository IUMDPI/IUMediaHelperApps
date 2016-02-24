using System;

namespace Packager.Exceptions
{
    public class UserCancelledException : AbstractEngineException
    {
        public UserCancelledException(Exception exception) : base(exception,"user canceled operation")
        {
        }

        public UserCancelledException() : base("user canceled operation")
        {
        }
    }
}