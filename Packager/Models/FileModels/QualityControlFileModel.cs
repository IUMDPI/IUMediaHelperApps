namespace Packager.Models.FileModels
{
    public class QualityControlFileModel : ObjectFileModel
    {
       
        public QualityControlFileModel(ObjectFileModel fileModel)
        {
            SequenceIndicator = fileModel.SequenceIndicator;
            Extension = ".qctools.xml.gz";
            FileUse = fileModel.FileUse;
            ProjectCode = fileModel.ProjectCode;
            BarCode = fileModel.BarCode;
        }

        public string ToIntermediateFileName()
        {
            return $"{ToFileNameWithoutExtension()}.qctools.xml";
        }
    }
}