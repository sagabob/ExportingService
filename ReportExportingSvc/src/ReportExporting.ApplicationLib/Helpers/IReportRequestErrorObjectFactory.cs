using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Helpers;

public interface IReportRequestErrorObjectFactory
{
    ReportRequestErrorObject CreateObjectErrorObject(string errorMessage);
}