namespace Packager.Attributes
{
    public class FromStringConfigSettingAttribute : AbstractFromConfigSettingAttribute
    {
        public FromStringConfigSettingAttribute(string name, bool required = true)
            : base(name, required)
        {
        }

        public override object Convert(string value)
        {
            return value;
        }
    }
}