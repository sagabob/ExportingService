using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Helpers.Core;

public class ReportRequestTableEntityFactory : IReportRequestTableEntityFactory
{
    public ReportRequestTableEntity CreateTableEntity(ReportRequestObject request)
    {
        return new ReportRequestTableEntity
        {
            FileName = request.FileName,
            EmailAddress = request.EmailAddress,
            PartitionKey = request.Id,
            RowKey = request.Id,
            Status = request.Status.ToString(),
            FullProgress = request.GetFullProgress()
        };
    }
}