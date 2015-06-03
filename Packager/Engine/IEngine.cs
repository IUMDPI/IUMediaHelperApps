using Packager.Observers;

namespace Packager.Engine
{
    public interface IEngine
    {
        void Start();
        void AddObserver(IObserver observer);
    }
}