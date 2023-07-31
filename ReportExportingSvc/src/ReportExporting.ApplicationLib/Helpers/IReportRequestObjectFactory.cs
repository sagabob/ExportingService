using ReportExporting.ApplicationLib.Entities;
using ReportExporting.Core;

namespace ReportExporting.ApplicationLib.Helpers;

public interface IReportRequestObjectFactory
{
    ReportRequestObject CreateFromReportRequest(ReportRequest request);
}