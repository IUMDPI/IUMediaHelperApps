using Packager.Extensions;

namespace Packager.Models
{
    public class BextData
    {
        public string Description { get; set; }
        public string IARL { get; set; }
        public string ICMT { get; set; }

        public string[] GenerateCommandArgs()
        {
            return new[]
            {
                string.Format("--Description={0}", Description.ToQuoted()),
                string.Format("--IARL={0}", IARL.ToQuoted()),
                string.Format("--ICMT={0}", ICMT.ToQuoted())
            };
        }
    }
}