namespace Packager.Exceptions
{
    public class UserCancelledException : AbstractEngineException
    {
        public UserCancelledException() : base("user canceled operation")
        {
        }
    }
}