using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Models;
using Reporter.Models;

namespace Reporter.Utilities
{
    public class ReportReader : IReportReader
    {
        private ProgramSettings ProgramSettings { get; }
        private string FolderPath => ProgramSettings.ReportFolder;
        public ReportReader(ProgramSettings programSettings)
        {
            ProgramSettings = programSettings;
        }

        public async Task<List<ReportEntry>> GetReports()
        {
            return (await Task.Run(() => Directory.EnumerateFiles(FolderPath, "Packager_*.report")))
                .Select(filename => new ReportEntry
                {
                    Filename = filename,
                    Timestamp = GetTicksFromFilename(filename),
                    DisplayName = GetDisplayNameFromFilename(filename)
                })
                .ToList();
        }

        private static long GetTicksFromFilename(string filename)
        {
            var withoutExtension = Path.GetFileNameWithoutExtension(filename).ToDefaultIfEmpty();
            var ticksText = withoutExtension.Split('_').Last();

            long result;
            return long.TryParse(ticksText, out result) 
                ? result 
                : 0;
        }

        private static string GetDisplayNameFromFilename(string filename)
        {
            var ticks = GetTicksFromFilename(filename);
            if (ticks <= 0)
            {
                return "Packager Session [unspecified date]";
            }

            return $"Packager Session {new DateTime(ticks):MM/dd/yyyy hh:mm tt}";
        }
        
        public async Task<T> GetReport<T>(ReportEntry reportEntry) where T: OperationReport
        {
            if (reportEntry == null)
            {
                return null;
            }

            return await Task.Run(() => 
                OperationReport.Read<T>(
                    Path.Combine(FolderPath, reportEntry.Filename)));
        }
    }
}