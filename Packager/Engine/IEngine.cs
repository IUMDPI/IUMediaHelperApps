using System.Threading;
using System.Threading.Tasks;
using Packager.Observers;

namespace Packager.Engine
{
    public interface IEngine
    {
        Task Start(CancellationToken cancellationToken);
        void AddObserver(IObserver observer);
    }
}