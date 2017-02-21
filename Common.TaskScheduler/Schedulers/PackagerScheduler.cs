namespace Common.TaskScheduler.Schedulers
{
    public class PackagerScheduler : AbstractScheduler
    {
        public PackagerScheduler() : base(
            Constants.PackagerIdentifier, 
            Constants.PackagerProductName)
        {
        }
    }
}
