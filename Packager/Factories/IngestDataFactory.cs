using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class IngestDataFactory : IIngestDataFactory
    {
        public IngestData Generate(ConsolidatedPodMetadata podMetadata, AbstractFileModel masterFileModel)
        {
            var digitalFileProvenance = podMetadata.FileProvenances.GetFileProvenance(masterFileModel);
            if (digitalFileProvenance == null)
            {
                throw new OutputXmlException("No digital file provenance found for {0}", masterFileModel.ToFileName());
            }

            return new IngestData
            {
                XsiType = $"{podMetadata.Format}Ingest".RemoveSpaces(),
                
                Comments = digitalFileProvenance.Comment,
                CreatedBy = digitalFileProvenance.CreatedBy,
                ExtractionWorkstation = digitalFileProvenance.ExtractionWorkstation,
                SpeedUsed = digitalFileProvenance.SpeedUsed,
                Date = digitalFileProvenance.DateDigitized.ToString(),
                Players = digitalFileProvenance.PlayerDevices
                    .Select(d=>new IngestDevice {Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer})
                    .ToArray(),
                AdDevices = digitalFileProvenance.AdDevices
                    .Select(d => new IngestDevice { Model = d.Model, SerialNumber = d.SerialNumber, Manufacturer = d.Manufacturer })
                    .ToArray()
            };

            
        }
    }
}