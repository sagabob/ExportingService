using Azure.Data.Tables;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Services
{
    public class TableStorageService
    {

        private readonly TableServiceClient _tableServiceClient;
        private readonly IConfiguration _configuration;
        public TableStorageService(TableServiceClient tableServiceClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _tableServiceClient = tableServiceClient;
        }

        private async Task<TableClient> GetTableClient()
        {
            var tableClient = _tableServiceClient.GetTableClient(_configuration["TableName"]);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }

        public async Task<ReportRequestEntity> GetEntityAsync(string category, string id)
        {
            var tableClient = await GetTableClient();
            return await tableClient.GetEntityAsync<ReportRequestEntity>(category, id);
        }

        public async Task<ReportRequestEntity> AddEntityAsync(ReportRequestEntity entity)
        {
            var tableClient = await GetTableClient();
            await tableClient.AddEntityAsync<ReportRequestEntity>(entity);
            return entity;
        }

        public async Task DeleteEntityAsync(string category, string id)
        {
            var tableClient = await GetTableClient();
            await tableClient.DeleteEntityAsync(category, id);

        }
    }
}
