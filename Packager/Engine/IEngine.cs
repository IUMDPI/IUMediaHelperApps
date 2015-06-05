using System.Threading.Tasks;
using Packager.Observers;

namespace Packager.Engine
{
    public interface IEngine
    {
        Task Start();
        void AddObserver(IObserver observer);
    }
}