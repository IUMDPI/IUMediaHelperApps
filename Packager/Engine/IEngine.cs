using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packager.Extensions;
using Packager.Models;
using Packager.Processors;
using Packager.Observers;
using Packager.Utilities;

namespace Packager.Engine
{
    public interface IEngine
    {
        void Start();
        void AddObserver(IObserver observer);
    }

    class StandardEngine : IEngine
    {
        private readonly IProgramSettings _programSettings;
        private readonly Dictionary<string, IProcessor> _processors;
        private readonly IUtilityProvider _utilityProvider;
        private readonly List<IObserver> _observers;
      
        public StandardEngine(IProgramSettings programSettings, 
            Dictionary<string, IProcessor> processors,
            IUtilityProvider utilityProvider,
            List<IObserver> observers)
        {
            _programSettings = programSettings;
            _processors = processors;
            _utilityProvider = utilityProvider;
            _observers = observers;
        }

        public void Start()
        {
            try
            {
                WriteHelloMessage();
                _programSettings.Verify();

                // want to get all files in the input directory
                // and convert them to file models
                // and then take all of the files that are valid
                // and start with the correct project code
                // and then group them by bar code
                var batchGroups = Directory.EnumerateFiles(_programSettings.InputDirectory)
                    .Select(p => new FileModel(p))
                    .Where(f => f.IsValidForGrouping())
                    .Where(f => f.BelongsToProject(_programSettings.ProjectCode))
                    .GroupBy(f => f.BarCode).ToList();

                // to do: catch exception and prompt user to retry, ignore, or cancel
                // if retry move group files back to input and start over
                foreach (var group in batchGroups)
                {
                    var processor = GetProcessor(group);
                    processor.ProcessFile(group);
                }

                WriteGoodbyeMessage();
            }
            catch (Exception ex)
            {
                _observers.Log("Fatal Exception Occurred: {0}", ex);
            }
        }

        public void AddObserver(IObserver observer)
        {
            if (_observers.Any(instance => instance.GetType() == observer.GetType()))
            {
                return;
            }

            _observers.Add(observer);
        }

        private IProcessor GetProcessor(IEnumerable<FileModel> group)
        {
            // for each model in the group
            // take those that have extensions associated with a processor
            // and group them by that extension
            var validExtensions = group
                .Where(m => _processors.Keys.Contains(m.Extension))
                .GroupBy(m => m.Extension).ToList();

            // if we have no groups or if we have more than one group, we have a problem
            if (validExtensions.Count() != 1)
            {
                throw new Exception("Can not determine extension for file batch");
            }

            // get processor for the group's common extension
            return _processors[validExtensions.First().Key];
        }

        private void WriteHelloMessage()
        {
            _observers.Log("Starting {0}", DateTime.Now);
        }

        private void WriteGoodbyeMessage()
        {
            _observers.Log("Completed {0}", DateTime.Now);
        }
    }
}