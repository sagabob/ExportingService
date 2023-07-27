using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Services;

namespace ReportExporting.PlaceOrderApi.Helpers;

public class DataFactory
{
    public static ReportRequestEntity CreateTableEntity(ReportRequest reportRequest)
    {
        var guidValue = reportRequest.Guid ?? Guid.NewGuid();
        return new ReportRequestEntity
        {
            FileName = reportRequest.FileName,
            EmailAddress = reportRequest.EmailAddress,
            PartitionKey = guidValue.ToString(),
            RowKey = guidValue.ToString(),
            Status = ExportingProgress.PutOnStore
        };
    }
}