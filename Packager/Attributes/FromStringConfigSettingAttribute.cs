namespace Packager.Attributes
{
    public class FromStringConfigSettingAttribute : AbstractFromConfigSettingAttribute
    {
        public FromStringConfigSettingAttribute(string name): base(name)
        {
        }

        public override object Convert(string value)
        {
            return value;
        }
    }
}