using Azure.Data.Tables;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Services;

public class TableStorageService : ITableStorageService
{
    private readonly IConfiguration _configuration;

    private readonly TableServiceClient _tableServiceClient;

    public TableStorageService(TableServiceClient tableServiceClient, IConfiguration configuration)
    {
        _configuration = configuration;
        _tableServiceClient = tableServiceClient;
    }

    public async Task<ReportRequestEntity> GetEntityAsync(string category, string id)
    {
        var tableClient = await GetTableClient();
        return await tableClient.GetEntityAsync<ReportRequestEntity>(category, id);
    }

    public async Task<ReportRequestEntity> AddEntityAsync(ReportRequestEntity entity)
    {
        var tableClient = await GetTableClient();
        try
        {
            var response = await tableClient.UpsertEntityAsync(entity);
            if (response.Status == 204)
            {
                var updatedEntity =
                    await tableClient.GetEntityAsync<ReportRequestEntity>(entity.PartitionKey, entity.RowKey);
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
        var tableClient = await GetTableClient();
        await tableClient.DeleteEntityAsync(category, id);
    }

    private async Task<TableClient> GetTableClient()
    {
        var tableClient = _tableServiceClient.GetTableClient(_configuration["TableName"]);
        await tableClient.CreateIfNotExistsAsync();
        return tableClient;
    }
}