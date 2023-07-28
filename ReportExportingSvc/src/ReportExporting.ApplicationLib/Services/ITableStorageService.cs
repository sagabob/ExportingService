using Azure;
using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Services;

public interface ITableStorageService
{
    Task<ReportRequestTableEntity> GetEntityAsync(string category, string id);
    Task<Response> AddEntityAsync(ReportRequestTableEntity entity);
    Task DeleteEntityAsync(string category, string id);
}