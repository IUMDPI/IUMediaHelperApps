using System.Collections.Specialized;
using Common.Extensions;

namespace Reporter.Models
{
    public class ProgramSettings
    {
        public ProgramSettings(NameValueCollection settings)
        {
            ProjectCode =  GetStringValue(settings, "ProjectCode");
        }

        public string ProjectCode { get; }

        public string ReportFolder {
            get { return Properties.Settings.Default.ReportFolder; }
            set
            {
                Properties.Settings.Default.ReportFolder = value;
                Properties.Settings.Default.Save();
            }
        }

        // todo: move to common
        private static string GetStringValue(NameValueCollection settings, string name)
        {
            return settings[name].ToDefaultIfEmpty().ToUpperInvariant();
        }
    }
}
