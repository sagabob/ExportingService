using Azure.Identity;
using Microsoft.Extensions.Azure;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Services;
using ReportExporting.ExportApi.Generators;
using ReportExporting.ExportApi.Handlers;
using ReportExporting.ProcessOrderApi.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAzureClients(cfg =>
{
    cfg.AddServiceBusClient(builder.Configuration.GetSection("ServiceBus"))
        .WithCredential(new DefaultAzureCredential());

    cfg.AddTableServiceClient(new Uri(builder.Configuration.GetValue<string>("TableStorageServiceUrl")!))
        .WithCredential(new DefaultAzureCredential());

    cfg.AddBlobServiceClient(new Uri(builder.Configuration.GetValue<string>("BlobStorageServiceUrl")!))
        .WithCredential(new DefaultAzureCredential());
});

builder.Services.AddSingleton<ITableStorageService, TableStorageService>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

builder.Services.AddSingleton<PdfReportGenerator>();
builder.Services.AddSingleton<WordReportGenerator>();
builder.Services.AddSingleton<IReportGeneratorService, ReportGeneratorFactory>();

builder.Services.AddSingleton<IExportRequestHandler, ExportRequestHandler>();
builder.Services.AddSingleton<IUpsertItemToTableHandler, UpsertItemToTableHandler>();
builder.Services.AddSingleton<IAddItemToQueueHandler, AddItemToQueueHandler>();
builder.Services.AddSingleton<IUploadItemToBlobHandler, UploadItemToBlobHandler>();

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

