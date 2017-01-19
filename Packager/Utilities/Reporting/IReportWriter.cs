using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Packager.Validators;

namespace Packager.Utilities.Reporting
{
    public interface IReportWriter
    {
        void WriteResultsReport(Dictionary<string, ValidationResult> results);
        void WriteResultsReport(Exception e);
    }
}
