using System;
using System.Collections.Generic;
using System.Linq;
using Packager.Extensions;

namespace Packager.Utilities.Process
{
    public class ArgumentBuilder : List<string>
    {
        public ArgumentBuilder()
        {
        }

        public ArgumentBuilder(string arguments)
        {
            AddRange(arguments.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries));
        }

        public ArgumentBuilder AddArguments(string arguments)
        {
            AddRange(arguments.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return this;
        }

        public ArgumentBuilder AddArguments(IEnumerable<string> arguments)
        {
            AddRange(arguments.Select(a=>a.ToDefaultIfEmpty().Trim()).Where(a=>!string.IsNullOrWhiteSpace(a)));
            return this;
        }
        
        public override string ToString()
        {
            return string.Join(" ", this);
        }
    }
}