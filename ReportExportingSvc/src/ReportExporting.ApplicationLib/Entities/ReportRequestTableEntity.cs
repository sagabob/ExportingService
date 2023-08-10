using Azure;
using Azure.Data.Tables;

namespace ReportExporting.ApplicationLib.Entities;

public class ReportRequestTableEntity : ITableEntity
{
    public string? FileName { get; set; }
    public required string EmailAddress { get; set; }
    public required string Status { get; set; }
    public required string FullProgress { get; set; }
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}