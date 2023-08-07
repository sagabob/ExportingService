using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Services.Core;

public class TableStorageService : ITableStorageService
{
    public TableStorageService(TableServiceClient tableServiceClient, IConfiguration configuration)
    {
        TableClient = tableServiceClient.GetTableClient(configuration["TableName"]);
    }

    public TableClient TableClient { get; }


    public async Task<Response> UpsertEntityAsync(ReportRequestTableEntity entity)
    {
        var response = await TableClient.UpsertEntityAsync(entity);

        return response;
    }
}