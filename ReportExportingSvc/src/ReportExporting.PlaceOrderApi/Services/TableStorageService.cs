using Azure.Data.Tables;

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
        var response = await tableClient.UpsertEntityAsync(entity);
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