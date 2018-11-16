using System;
using System.Collections.Generic;
using System.Linq;
using Packager.Extensions;

namespace Packager.Utilities.ProcessRunners
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
            AddRange(arguments.Select(a=>a.ToDefaultIfEmpty().Trim()).Where(a=>a.IsSet()));
            return this;
        }

        public ArgumentBuilder Clone()
        {
            return new ArgumentBuilder()
                .AddArguments(this);
        }
        
        public override string ToString()
        {
            return string.Join(" ", this);
        }
    }
}