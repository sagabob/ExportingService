using System.Reflection;
using Azure.Identity;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Azure;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Handlers.Core;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ApplicationLib.Services;
using ReportExporting.ApplicationLib.Services.Core;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Handlers;
using ReportExporting.PlaceOrderApi.Handlers.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddValidatorsFromAssemblyContaining<ExportRequestValidator>();

builder.Services.AddAzureClients(cfg =>
{
    cfg.AddServiceBusClient(builder.Configuration.GetSection("ServiceBus"))
        .WithCredential(new DefaultAzureCredential());

    cfg.AddTableServiceClient(new Uri(builder.Configuration.GetValue<string>("TableStorageServiceUrl")!))
        .WithCredential(new DefaultAzureCredential());
});

builder.Services.AddScoped<IReportRequestObjectFactory, ReportRequestObjectFactory>();
builder.Services.AddScoped<IReportRequestTableEntityFactory, ReportRequestTableEntityFactory>();



builder.Services.AddScoped<ITableStorageService, TableStorageService>();

builder.Services.AddScoped<IUpsertItemToTableHandler, UpsertItemToTableHandler>();
builder.Services.AddScoped<IAddItemToQueueHandler, AddItemToQueueHandler>();
builder.Services.AddScoped<IExportRequestHandler, ExportRequestHandler>();

var app = builder.Build();

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