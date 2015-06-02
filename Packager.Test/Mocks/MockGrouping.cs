using System.Collections.Generic;
using System.Linq;
using Packager.Models.FileModels;

namespace Packager.Test.Mocks
{
    public class MockFileGrouping : List<AbstractFileModel>, IGrouping<string, AbstractFileModel>
    {
        public string Key { get; set; }
    }
}