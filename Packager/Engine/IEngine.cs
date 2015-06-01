using System;
using System.Collections.Generic;
using System.Linq;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;

namespace Packager.Engine
{
    public interface IEngine
    {
        void Start();
        void AddObserver(IObserver observer);
    }

    public class StandardEngine : IEngine
    {
        private readonly List<IObserver> _observers;
        private readonly Dictionary<string, IProcessor> _processors;
        private readonly IProgramSettings _programSettings;
        private readonly IDependencyProvider _utilityProvider;

        public StandardEngine(IProgramSettings programSettings,
            Dictionary<string, IProcessor> processors,
            IDependencyProvider utilityProvider,
            List<IObserver> observers)
        {
            _programSettings = programSettings;
            _processors = processors;
            _utilityProvider = utilityProvider;
            _observers = observers;
        }

        private IDirectoryProvider DirectoryProvider
        {
            get { return _utilityProvider.DirectoryProvider; }
        }

        public void Start()
        {
            try
            {
                WriteHelloMessage();
                _programSettings.Verify();

                // this factory will assign each extension
                // to the appropriate file model
                var factory = new FileModelFactory(_processors.Keys);

                // want to get all files in the input directory
                // and convert them to file models (via out file model factory)
                // and then take all of the files that are valid
                // and start with the correct project code
                // and then group them by barcode
                var batchGroups = DirectoryProvider.EnumerateFiles(_programSettings.InputDirectory)
                    .Select(p => factory.GetModel(p))
                    .Where(f => f.IsValid())
                    .Where(f => f.BelongsToProject(_programSettings.ProjectCode))
                    .GroupBy(f => f.BarCode).ToList();

                // todo: catch exception and prompt user to retry, ignore, or cancel
                // todo: if retry move group files back to input and start over
                // now we want to get the processor for each group
                // and process the files for the group
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

        private IProcessor GetProcessor(IEnumerable<AbstractFileModel> group)
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