using System.Collections.Generic;
using Common.Models;
using Packager.Exceptions;
using Packager.Utilities.ProcessRunners;

namespace Packager.Factories.FFMPEGArguments
{
    public class FFMPEGArgumentsFactory : IFFMPEGArgumentsFactory
    {
        private Dictionary<IMediaFormat, IFFMPEGArgumentsGenerator> Generators { get; }

        public FFMPEGArgumentsFactory(Dictionary<IMediaFormat, IFFMPEGArgumentsGenerator> generators)
        {
            Generators = generators;
        }
        
        public IFFMPEGArgumentsGenerator GetGenerator(IMediaFormat format)
        {
            if (!Generators.ContainsKey(format))
            {
                throw new FormatNotSupportedException(format);
            }

            return Generators[format];
        }

        public ArgumentBuilder GetAccessArguments(IMediaFormat format) => 
            GetGenerator(format).GetAccessArguments();

        public ArgumentBuilder GetNormalizingArguments(IMediaFormat format) =>
            GetGenerator(format).GetNormalizingArguments();

        public ArgumentBuilder GetProdOrMezzArguments(IMediaFormat format) => 
            GetGenerator(format).GetProdOrMezzArguments();
    }
}