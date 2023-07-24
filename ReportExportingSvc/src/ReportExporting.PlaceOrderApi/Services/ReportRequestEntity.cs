using Azure;
using Azure.Data.Tables;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Services;

public class ReportRequestEntity : ITableEntity
{
    public string FileName { get; set; }
    public string EmailAddress { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public ExportingProgress Status { get; set; } 
}