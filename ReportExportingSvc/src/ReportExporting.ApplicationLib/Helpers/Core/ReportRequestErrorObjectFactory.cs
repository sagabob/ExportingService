using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Helpers.Core;

public class ReportRequestErrorObjectFactory : IReportRequestErrorObjectFactory
{
    public ReportRequestErrorObject CreateObjectErrorObject(string errorMessage)
    {
        return new ReportRequestErrorObject
        {
            Id = "Error" + Guid.NewGuid(),
            ErrorMassage = errorMessage
        };
    }
}