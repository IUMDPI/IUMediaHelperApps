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
                string.Format("--Description=\"{0}\"", Description),
                string.Format("--IARL=\"{0}\"", IARL),
                string.Format("--ICMT=\"{0}\"", ICMT)
            };
        }
    }
}