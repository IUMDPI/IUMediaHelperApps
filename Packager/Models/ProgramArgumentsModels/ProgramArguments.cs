using System;
using System.Linq;

namespace Packager.Models.ProgramArgumentsModels
{
    public class ProgramArguments : IProgramArguments
    {
        public ProgramArguments(string[] arguments)
        {
            Interactive = arguments.Any(a => a.Equals("-noninteractive", StringComparison.InvariantCultureIgnoreCase))==false;
        }

        public bool Interactive { get; }
    }
}