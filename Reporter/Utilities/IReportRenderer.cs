using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;
using Common.UserInterface.ViewModels;
using Reporter.Models;

namespace Reporter.Utilities
{
    public interface IReportRenderer
    {
        Task<List<AbstractReportEntry>> GetReports();
        Task Render(AbstractReportEntry reportEntry);
        bool CanRender(AbstractReportEntry report);
    }
}
