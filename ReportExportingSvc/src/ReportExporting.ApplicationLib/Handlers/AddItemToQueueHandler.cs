using System.Text;
using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Messages;

namespace ReportExporting.ApplicationLib.Handlers;

public class AddItemToQueueHandler : IRequestHandler<AddItemToQueueRequest, ReportRequestObject>
{
    private readonly ServiceBusSender _serviceBusSender;

    public AddItemToQueueHandler(ServiceBusClient serviceBusClient, IConfiguration configuration)
    {
        _serviceBusSender = serviceBusClient.CreateSender(configuration["QueueName"]);
    }

    public async Task<ReportRequestObject> Handle(AddItemToQueueRequest request, CancellationToken cancellationToken)
    {
        if (request.PayLoad.Status == ExportingStatus.Failure)
            return request.PayLoad;

        request.PayLoad.Progress.Add(ExportingProgress.PlaceOnQueue);
        try
        {
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request.PayLoad)))
            {
                ContentType = "application/json",
                MessageId = request.PayLoad.Id.ToString()
            };

            await _serviceBusSender.SendMessageAsync(message, cancellationToken);
        }
        catch (Exception)
        {
            request.PayLoad.Status = ExportingStatus.Failure;
            request.PayLoad.Progress.Add(ExportingProgress.FailToPlaceOnQueue);
        }

        return request.PayLoad;
    }
}