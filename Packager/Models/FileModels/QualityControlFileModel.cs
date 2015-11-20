using System.IO;

namespace Packager.Models.FileModels
{
    public class QualityControlFileModel : AbstractObjectFileModel
    {
        public QualityControlFileModel(AbstractObjectFileModel fileModel)
        {
            SequenceIndicator = fileModel.SequenceIndicator;
            Extension = ".gz";
            FileUse = fileModel.FileUse;
            ProjectCode = fileModel.ProjectCode;
            BarCode = fileModel.BarCode;
        }

        public string ToIntermediateFileName()
        {
            return $"{ToFileNameWithoutExtension()}.xml";
        }
    }
}