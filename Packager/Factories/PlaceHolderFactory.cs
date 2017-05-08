using System.Collections.Generic;
using Common.Models;
using Packager.Models.FileModels;
using Packager.Models.PlaceHolderConfigurations;
using Packager.Observers;

namespace Packager.Factories
{
    public class PlaceHolderFactory : IPlaceHolderFactory
    {
        public PlaceHolderFactory(
            Dictionary<IMediaFormat, IPlaceHolderConfiguration> knownConfigurations, 
            IObserverCollection observers)
        {
            _knownConfigurations = knownConfigurations;
            _observers = observers;
        }

        private readonly Dictionary<IMediaFormat, IPlaceHolderConfiguration> _knownConfigurations;
        private readonly IObserverCollection _observers;

        public List<AbstractFile> GetPlaceHoldersToAdd(IMediaFormat format, List<AbstractFile> fileModels)
        {
            if (!_knownConfigurations.ContainsKey(format))
            {
                _observers.Log("No placeholder configuration found for format {0}", format.ProperName);
                return new List<AbstractFile>();
            }

            return _knownConfigurations[format]
                .GetPlaceHoldersToAdd(fileModels);
        }

       
    }
}