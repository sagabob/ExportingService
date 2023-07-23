using Azure.Data.Tables;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Services
{
    public class TableStorageService
    { 

        private readonly IConfiguration _configuration;
        public TableStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<TableClient> GetTableClient()
        {
            var serviceClient = new TableServiceClient(_configuration["StorageConnectionString"]);
            var tableClient = serviceClient.GetTableClient(_configuration["TableName"]);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }

        public async Task<ReportRequest> GetEntityAsync(string category, string id)
        {
            var tableClient = await GetTableClient();
            return await tableClient.GetEntityAsync<ReportRequest>(category, id);
        }
        public async Task<ReportRequest> AddEntityAsync(ReportRequest entity)
        {
            var tableClient = await GetTableClient();
            await tableClient.AddEntityAsync<ReportRequest>(entity);
            return entity;
        }
        public async Task DeleteEntityAsync(string category, string id)
        {
            var tableClient = await GetTableClient();
            await tableClient.DeleteEntityAsync(category, id);

        }
    }
