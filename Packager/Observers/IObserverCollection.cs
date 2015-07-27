using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packager.Observers
{
    public interface IObserverCollection: IList<IObserver>
    {
        void Log(string baseMessage, params object[] elements);
        void LogIssue(Exception issue);
        Guid BeginSection(string baseMessage, params object[] elements);
        void EndSection(Guid sectionKey, string newTitle = "", bool collapse = false);
    }
}
