namespace Packager.Models.FileModels
{
    public class PlaceHolderFile : UnknownFile
    {
        public PlaceHolderFile(string projectCode, string barcode, int sequenceIndicator) 
            : base($"{projectCode}_{barcode}_{sequenceIndicator}")
        {
            PlaceHolder = true;
        }
    }
}