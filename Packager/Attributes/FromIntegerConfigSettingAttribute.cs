namespace Packager.Attributes
{
    public class FromIntegerConfigSettingAttribute : AbstractFromConfigSettingAttribute
    {
        public FromIntegerConfigSettingAttribute(string name): base(name)
        {
        }


        public override object Convert(string value)
        {
            int convertedValue;
            if (int.TryParse(value, out convertedValue) == false)
            {
                Issues.Add($"\"{value}\" is not a valid integer value");
                return null;
            }

            return convertedValue;
        }
    }
}