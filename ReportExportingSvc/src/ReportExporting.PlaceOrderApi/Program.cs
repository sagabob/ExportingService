using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using ReportExporting.PlaceOrderApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var tableStorageEndpoint = builder.Configuration.GetValue<string>("TableStorageServiceUrl");

builder.Services.AddSingleton(x =>
    new TableServiceClient(
        new Uri(tableStorageEndpoint!),
        new DefaultAzureCredential()));

builder.Services.AddScoped<ITableStorageService, TableStorageService>();

builder.Services.AddAzureClients(cfg =>
{
    cfg.AddServiceBusClient(builder.Configuration.GetSection("ServiceBus"))
        .WithCredential(new DefaultAzureCredential());
});


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