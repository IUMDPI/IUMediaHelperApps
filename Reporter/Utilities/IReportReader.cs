using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;
using Reporter.Models;

namespace Reporter.Utilities
{
    public interface IReportReader
    {
        Task<List<ReportEntry>> GetReports();
        Task<T> GetReport<T>(ReportEntry reportEntry) where T:OperationReport;
    }
}
