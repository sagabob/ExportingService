using Azure;
using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Services;

public interface ITableStorageService
{
    Task<Response> UpsertEntityAsync(ReportRequestTableEntity entity);
    
}