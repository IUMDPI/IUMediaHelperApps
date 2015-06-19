using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Processors
{
    public interface IProcessor
    {
        Task<bool> ProcessFile(IEnumerable<AbstractFileModel> fileModels);
        Guid SectionKey { get; }
        string Barcode { get; }
    }
}