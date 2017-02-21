using System;
using System.IO;
using System.Security;
using Common.TaskScheduler.Extensions;
using Microsoft.Win32.TaskScheduler;

namespace Common.TaskScheduler.Schedulers
{
    public class PackagerScheduler : AbstractScheduler
    {
        public PackagerScheduler() : base(
            Constants.PackagerTaskName,
            Constants.PackagerIdentifier, 
            Constants.PackagerProductName)
        {
        }

        

        
    }
}
