using System.Collections.Generic;
using System.Linq;
using Packager.Models.FileModels;

namespace Packager.Test.Mocks
{
    public class MockFileGrouping : List<AbstractFile>, IGrouping<string, AbstractFile>
    {
        public string Key { get; set; }
    }
}