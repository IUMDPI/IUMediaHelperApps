namespace Packager.Models.PodMetadataModels
{
    public class PodDataObjectTechnicalMetadataTapeThickness
    {
        public bool OnePoint5Mils { get; set; }
        public bool OneMils { get; set; }

        public string GetValue()
        {
            if (OneMils)
            {
                return "1";
            }

            if (OnePoint5Mils)
            {
                return "1.5";
            }

            return string.Empty;
        }
    }
}