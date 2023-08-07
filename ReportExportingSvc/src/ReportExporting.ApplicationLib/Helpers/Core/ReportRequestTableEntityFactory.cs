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
            PartitionKey = request.Id.ToString(),
            RowKey = request.Id.ToString(),
            Status = Enum.GetName(typeof(ExportingStatus), request.Status),
            FullProgress = request.GetFullProgress()
        };
    }
}