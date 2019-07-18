namespace Packager.Models.FileModels
{
    public class PlaceHolderFile : UnknownFile
    {
        public PlaceHolderFile(string projectCode, string barcode, int sequenceIndicator, string extension) 
            : base($"{projectCode}_{barcode}_{sequenceIndicator}_pres.{extension}")
        {
            IsPlaceHolder = true;
        }
    }
}