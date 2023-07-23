using Azure;
using Azure.Data.Tables;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Services;

public class ReportRequestEntity : ITableEntity
{
    public ReportRequestEntity(ReportRequest inputRequest)
    {
        Title = inputRequest.Title;
        Urls = inputRequest.Urls;
        inputRequest.Guid ??= Guid.NewGuid();
        
        PartitionKey = inputRequest.Guid.ToString()!;
        RowKey = inputRequest.Guid.ToString()!;
    }
    public ReportUrl[] Urls { get; set; } 
    public string Title { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}