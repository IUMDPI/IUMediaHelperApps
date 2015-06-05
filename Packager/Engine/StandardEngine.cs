using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;

namespace Packager.Engine
{
    public class StandardEngine : IEngine
    {
        private  List<IObserver> Observers {get { return _dependencyProvider.Observers; }}
        private readonly Dictionary<string, IProcessor> _processors;
        private IProgramSettings ProgramSettings {get { return _dependencyProvider.ProgramSettings; }}
        private readonly IDependencyProvider _dependencyProvider;

        public StandardEngine(
            Dictionary<string, IProcessor> processors,
            IDependencyProvider dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
            _processors = processors;
            
        }

        private IDirectoryProvider DirectoryProvider
        {
            get { return _dependencyProvider.DirectoryProvider; }
        }

        public async Task Start()
        {
            try
            {
                WriteHelloMessage();
                ProgramSettings.Verify();

                // this factory will assign each extension
                // to the appropriate file model
                var factory = new FileModelFactory(_processors.Keys);

                // want to get all files in the input directory
                // and convert them to file models (via out file model factory)
                // and then take all of the files that are valid
                // and start with the correct project code
                // and then group them by barcode
                var batchGroups = DirectoryProvider.EnumerateFiles(ProgramSettings.InputDirectory)
                    .Select(p => factory.GetModel(p))
                    .Where(f => f.IsValid())
                    .Where(f => f.BelongsToProject(ProgramSettings.ProjectCode))
                    .GroupBy(f => f.BarCode).ToList();

                // todo: catch exception and prompt user to retry, ignore, or cancel
                // todo: if retry move group files back to input and start over
                // now we want to get the processor for each group
                // and process the files for the group
                foreach (var group in batchGroups)
                {
                    var processor = GetProcessor(group);
                    await processor.ProcessFile(group);
                }

                WriteGoodbyeMessage();
            }
            catch (Exception ex)
            {
                Observers.Log("Fatal Exception Occurred: {0}", ex);
            }
        }

        public void AddObserver(IObserver observer)
        {
            if (Observers.Any(instance => instance.GetType() == observer.GetType()))
            {
                return;
            }

            Observers.Add(observer);
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
            Observers.Log("Starting {0}", DateTime.Now);
        }

        private void WriteGoodbyeMessage()
        {
            Observers.Log("Completed {0}", DateTime.Now);
        }
    }
}