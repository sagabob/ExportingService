using Azure.Identity;
using Microsoft.Extensions.Azure;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Handlers.Core;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ApplicationLib.Services;
using ReportExporting.ApplicationLib.Services.Core;
using ReportExporting.NotificationApi.Handlers;
using ReportExporting.NotificationApi.Handlers.Core;
using ReportExporting.NotificationApi.Helpers;
using ReportExporting.NotificationApi.Helpers.Core;
using ReportExporting.NotificationApi.Services;
using ReportExporting.NotificationApi.Services.Core;
using SendGrid.Extensions.DependencyInjection;

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

builder.Services.AddSendGrid(options =>
{
    options.ApiKey = builder.Configuration
        .GetSection("SendGridEmailSettings").GetValue<string>("APIKey");
});

builder.Services.AddSingleton<IReportRequestTableEntityFactory, ReportRequestTableEntityFactory>();
builder.Services.AddSingleton<IReportRequestErrorObjectFactory, ReportRequestErrorObjectFactory>();

builder.Services.AddSingleton<ITableStorageService, TableStorageService>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

builder.Services.AddSingleton<IEmailContentHelpers, EmailContentHelpers>();
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddSingleton<IUpsertItemToTableHandler, UpsertItemToTableHandler>();
builder.Services.AddSingleton<IAddItemToQueueHandler, AddItemToQueueHandler>();
builder.Services.AddSingleton<IDownloadItemFromBlobHandler, DownloadItemFromBlobHandler>();
builder.Services.AddSingleton<IAddItemToQueueHandler, AddItemToQueueHandler>();
builder.Services.AddSingleton<ISendEmailHandler, SendEmailHandler>();


builder.Services.AddSingleton<IMessageForEmailHandler, MessageForEmailHandler>();

var app = builder.Build();

app.Services.GetService<IMessageForEmailHandler>()?.Register();

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