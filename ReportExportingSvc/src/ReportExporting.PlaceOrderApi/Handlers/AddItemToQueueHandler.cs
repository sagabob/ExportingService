using System.Text;
using Azure.Messaging.ServiceBus;
using MediatR;
using Newtonsoft.Json;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Requests;

namespace ReportExporting.PlaceOrderApi.Handlers;

public class AddItemToQueueHandler : IRequestHandler<AddItemToQueueRequest, ReportRequest>
{
    private readonly ServiceBusSender _serviceBusSender;

    public AddItemToQueueHandler(ServiceBusClient serviceBusClient, IConfiguration configuration)
    {
        _serviceBusSender = serviceBusClient.CreateSender(configuration["QueueName"]);
    }

    public async Task<ReportRequest> Handle(AddItemToQueueRequest request, CancellationToken cancellationToken)
    {
        request.PayLoad.Status = ExportingProgress.PlaceOnQueue;
        try
        {
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request.PayLoad)))
            {
                ContentType = "application/json",
                MessageId = request.PayLoad.Guid.ToString()
            };

            await _serviceBusSender.SendMessageAsync(message, cancellationToken);
        }
        catch (Exception)
        {
            request.PayLoad.Status = ExportingProgress.FailToPlaceOnQueue;
        }

        return request.PayLoad;
    }
}