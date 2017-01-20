using System;
using System.Collections.Generic;
using Packager.Validators;

namespace Packager.Utilities.Reporting
{
    public interface IReportWriter
    {
        void WriteResultsReport(Dictionary<string, DurationResult> results, DateTime startTime);
        void WriteResultsReport(Exception e);
    }
}
