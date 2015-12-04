using System.Collections.Generic;

namespace Packager.Observers
{
    internal class ObserverCollectionEqualityComparer : IEqualityComparer<IObserver>
    {
        public bool Equals(IObserver x, IObserver y)
        {
            return x.GetType() == y.GetType();
        }

        public int GetHashCode(IObserver obj)
        {
            return obj.UniqueIdentifier;
        }
    }
}