using Azure.Data.Tables;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Services;

public class TableStorageService : ITableStorageService
{
    public TableStorageService(TableServiceClient tableServiceClient, IConfiguration configuration)
    {
        TableClient = tableServiceClient.GetTableClient(configuration["TableName"]);
        TableClient.CreateIfNotExists();
    }

    public TableClient TableClient { get; }


    public async Task<ReportRequestEntity> GetEntityAsync(string category, string id)
    {
        return await TableClient.GetEntityAsync<ReportRequestEntity>(category, id);
    }

    public async Task<ReportRequestEntity> AddEntityAsync(ReportRequestEntity entity)
    {
        try
        {
            var response = await TableClient.UpsertEntityAsync(entity);
            if (response.Status == 204)
            {
                var updatedEntity =
                    await TableClient.GetEntityAsync<ReportRequestEntity>(entity.PartitionKey, entity.RowKey);
                return updatedEntity;
            }
        }
        catch (Exception)
        {
            entity.Status = ExportingProgress.FailToPutOnStore;
        }

        return entity;
    }

    public async Task DeleteEntityAsync(string category, string id)
    {
        await TableClient.DeleteEntityAsync(category, id);
    }
    
}