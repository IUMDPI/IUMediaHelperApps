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
    public interface IReportReader
    {
        Task<List<ReportEntry>> GetReports(string folderPath);
        Task<T> GetReport<T>(string folderPath, ReportEntry reportEntry) where T:OperationReport;
    }

    public class ReportReader : IReportReader
    {
        public async Task<List<ReportEntry>> GetReports(string folderPath)
        {
            return (await Task.Run(() => Directory.EnumerateFiles(folderPath, "Packager_*.report")))
                .Select(filename => new ReportEntry
                {
                    Filename = filename,
                    Timestamp = GetTicksFromFilename(filename),
                    DisplayName = GetDisplayNameFromFilename(filename)
                })
            .ToList();
        }

        private long GetTicksFromFilename(string filename)
        {
            var withoutExtension = Path.GetFileNameWithoutExtension(filename).ToDefaultIfEmpty();
            var ticksText = withoutExtension.Split('_').Last();

            long result;
            return long.TryParse(ticksText, out result) 
                ? result 
                : 0;
        }

        private string GetDisplayNameFromFilename(string filename)
        {
            var ticks = GetTicksFromFilename(filename);
            if (ticks <= 0)
            {
                return "Packager Session [unspecified date]";
            }

            return $"Packager Session {new DateTime(ticks):MM/dd/yyyy hh:mm tt}";
        }
        
        public async Task<T> GetReport<T>(string folderPath, ReportEntry reportEntry) where T: OperationReport
        {
            if (reportEntry == null)
            {
                return null;
            }

            return await Task.Run(() => 
                OperationReport.Read<T>(
                    Path.Combine(folderPath, reportEntry.Filename)));
        }
    }
}
