using System;
using System.Collections.Generic;
using System.IO;

namespace Packager.Models.FileModels
{
    public class FileModelFactory
    {
        private readonly Dictionary<string, Func<string, AbstractFileModel>> _generators;

        public FileModelFactory(IEnumerable<string> processorExtensions)
        {
            var generators = new Dictionary<string, Func<string, AbstractFileModel>>();
            foreach (var processorExtension in processorExtensions)
            {
                generators[processorExtension] = path => new ObjectFileModel(path);
            }

            generators[".xlsx"] = path => new ExcelFileModel(path);

            _generators = generators;
        }

        public AbstractFileModel GetModel(string path)
        {
            if (!Path.HasExtension(path))
            {
                return new UnknownFileModel(path);
            }

            var extension = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(extension))
            {
                return new UnknownFileModel(path);
            }

            return _generators.ContainsKey(extension)
                ? _generators[extension](path)
                : new UnknownFileModel(path);
        }
    }
}
