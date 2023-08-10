using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Handlers.Core;

public class AddItemToQueueHandler : IAddItemToQueueHandler
{
    private readonly IConfiguration _configuration;
    private readonly ServiceBusClient _serviceBusClient;

    public AddItemToQueueHandler(ServiceBusClient serviceBusClient, IConfiguration configuration)
    {
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
    }

    public async Task<ReportRequestObject> Handle(ReportRequestObject request, QueueType queueType)
    {
        if (request.Status == ExportingStatus.Failure && queueType == QueueType.WorkQueue)
            return request;

        try
        {
            var serviceBusSender = _serviceBusClient.CreateSender(_configuration[queueType.ToString()]);

            request.Progress.Add(queueType == QueueType.WorkQueue
                ? ExportingProgress.PlaceOrderOnQueue
                : ExportingProgress.SendOrderToEmailQueue);

            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request)))
            {
                ContentType = "application/json",
                MessageId = request.Id
            };

            await serviceBusSender.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            request.Status = ExportingStatus.Failure;
            request.Progress.Add(queueType == QueueType.WorkQueue
                ? ExportingProgress.FailToPlaceOrderOnQueue
                : ExportingProgress.FailSendingOrderToEmailQueue);
            request.ErrorMessage = ex.Message;
        }

        return request;
    }
}