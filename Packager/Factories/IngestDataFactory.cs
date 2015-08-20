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
                XsiType = string.Format("{0}Ingest", podMetadata.Format),
                AdManufacturer = digitalFileProvenance.AdManufacturer,
                AdModel = digitalFileProvenance.AdModel,
                AdSerialNumber = digitalFileProvenance.AdSerialNumber,
                Comments = digitalFileProvenance.Comment,
                CreatedBy = digitalFileProvenance.CreatedBy,
                PlayerManufacturer = digitalFileProvenance.PlayerManufacturer,
                PlayerModel = digitalFileProvenance.PlayerModel,
                PlayerSerialNumber = digitalFileProvenance.PlayerSerialNumber,
                ExtractionWorkstation = digitalFileProvenance.ExtractionWorkstation,
                SpeedUsed = digitalFileProvenance.SpeedUsed,
                Date = digitalFileProvenance.DateDigitized.ToString(), 
                PreAmp = digitalFileProvenance.PreAmp, 
                PreAmpSerialNumber = digitalFileProvenance.PreAmpSerialNumber
            };
        }
    }
}