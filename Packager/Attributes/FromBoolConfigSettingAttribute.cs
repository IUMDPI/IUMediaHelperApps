namespace Packager.Attributes
{
    public class FromBoolConfigSettingAttribute : AbstractFromConfigSettingAttribute
    {
        public FromBoolConfigSettingAttribute(string name, bool required = true)
            : base(name, required)
        {
        }

        public override object Convert(string value)
        {
            bool convertedValue;
            if (bool.TryParse(value, out convertedValue) == false)
            {
                Issues.Add($"\"{value}\" is not a valid boolean value");
                return null;
            }

            return convertedValue;
        }
    }
}