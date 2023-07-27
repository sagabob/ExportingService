using Azure.Identity;
using Microsoft.Extensions.Azure;
using ReportExporting.ExportApi.Generators;
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
});

builder.Services.AddScoped<PdfReportGenerator>();
builder.Services.AddScoped<WordReportGenerator>();
builder.Services.AddScoped<IReportGeneratorService, ReportGeneratorFactory>();
builder.Services.AddScoped<MessageHandler>();

var app = builder.Build();


app.Services.GetService<MessageHandler>()?.Register();

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