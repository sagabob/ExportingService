using Azure.Identity;
using Microsoft.Extensions.Azure;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Handlers.Core;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ApplicationLib.Services;
using ReportExporting.ApplicationLib.Services.Core;
using ReportExporting.ExportApi.Generators;
using ReportExporting.ExportApi.Generators.Core;
using ReportExporting.ExportApi.Handlers;
using ReportExporting.ExportApi.Handlers.Core;
using ReportExporting.ExportApi.Models;
using ReportExporting.ExportApi.Models.Core;
using ReportExporting.ProcessOrderApi.Handlers;
using ReportExporting.ProcessOrderApi.Handlers.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var azureCredential = new ClientSecretCredential(builder.Configuration.GetValue<string>("AZURE_TENANT_ID"),
    builder.Configuration.GetValue<string>("AZURE_CLIENT_ID"),
    builder.Configuration.GetValue<string>("AZURE_CLIENT_SECRET"));

builder.Services.AddAzureClients(cfg =>
{
    cfg.AddServiceBusClient(builder.Configuration.GetSection("ServiceBus"))
        .WithCredential(azureCredential);

    cfg.AddTableServiceClient(new Uri(builder.Configuration.GetValue<string>("TableStorageServiceUrl")!))
        .WithCredential(azureCredential);

    cfg.AddBlobServiceClient(new Uri(builder.Configuration.GetValue<string>("BlobStorageServiceUrl")!))
        .WithCredential(azureCredential);
});

builder.Services.AddSingleton<IReportRequestObjectFactory, ReportRequestObjectFactory>();
builder.Services.AddSingleton<IReportRequestTableEntityFactory, ReportRequestTableEntityFactory>();
builder.Services.AddSingleton<IReportRequestErrorObjectFactory, ReportRequestErrorObjectFactory>();


builder.Services.AddSingleton<ITableStorageService, TableStorageService>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();


builder.Services.AddSingleton<IReportGenerator, ReportGenerator>();


builder.Services.AddSingleton<IExportObjectFactory, ExportObjectFactory>();
builder.Services.AddSingleton<IExportConfigurationFactory, ExportConfigurationFactory>();
builder.Services.AddSingleton<IReportGeneratorFactory, ReportGeneratorFactory>();

builder.Services.AddSingleton<IExportRequestHandler, ExportRequestHandler>();
builder.Services.AddSingleton<IAddErrorItemToQueueHandler, AddErrorItemToQueueHandler>();
builder.Services.AddSingleton<IUpsertItemToTableHandler, UpsertItemToTableHandler>();
builder.Services.AddSingleton<IAddItemToQueueHandler, AddItemToQueueHandler>();
builder.Services.AddSingleton<IUploadItemToBlobHandler, UploadItemToBlobHandler>();
builder.Services.AddSingleton<IAddItemToQueueHandler, AddItemToQueueHandler>();


builder.Services.AddSingleton<IHandleExportProcess, HandleExportProcess>();
builder.Services.AddSingleton<IMessageHandler, MessageHandler>();

var app = builder.Build();


app.Services.GetService<IMessageHandler>()?.Register();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();