namespace ReportExporting.PlaceOrderApi.Services
{
    public interface ITableStorageService
    {
        Task<ReportRequestEntity> GetEntityAsync(string category, string id);
        Task<ReportRequestEntity> AddEntityAsync(ReportRequestEntity entity);
        Task DeleteEntityAsync(string category, string id);
    }

}
