using Azure.Identity;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.NotificationApi.Handlers;
using SendGrid.Extensions.DependencyInjection;

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

builder.Services.AddSendGrid(options =>
{
    options.ApiKey = builder.Configuration
        .GetSection("SendGridEmailSettings").GetValue<string>("APIKey");
});


builder.Services.AddSingleton<IUpsertItemToTableHandler, UpsertItemToTableHandler>();
builder.Services.AddSingleton<IAddItemToQueueHandler, AddItemToQueueHandler>();
builder.Services.AddSingleton<IDownloadItemFromBlobHandler, DownloadItemFromBlobHandler>();
builder.Services.AddSingleton<IAddItemToQueueHandler, AddItemToQueueHandler>();

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