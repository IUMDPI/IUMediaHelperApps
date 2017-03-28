using System.Collections.Generic;
using Packager.Models.FileModels;
using Packager.Models.PlaceHolderConfigurations;
using Packager.Observers;

namespace Packager.Factories
{
    public class PlaceHolderFactory : IPlaceHolderFactory
    {
        public PlaceHolderFactory(Dictionary<string, IPlaceHolderConfiguration> knownConfigurations, IObserverCollection observers)
        {
            _knownConfigurations = knownConfigurations;
            _observers = observers;
        }

        private readonly Dictionary<string, IPlaceHolderConfiguration> _knownConfigurations;
        private readonly IObserverCollection _observers;

        public List<AbstractFile> GetPlaceHoldersToAdd(string format, List<AbstractFile> fileModels)
        {
            if (!_knownConfigurations.ContainsKey(format.ToLowerInvariant()))
            {
                _observers.Log("No placeholder configuration found for format {0}", format);
                return new List<AbstractFile>();
            }

            return _knownConfigurations[format.ToLowerInvariant()]
                .GetPlaceHoldersToAdd(fileModels);
        }

       
    }
}