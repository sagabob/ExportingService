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
    private readonly IConfiguration _configuration;
    private readonly ServiceBusClient _serviceBusClient;

    public AddItemToQueueHandler(ServiceBusClient serviceBusClient, IConfiguration configuration)
    {
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
    }

    public async Task<ReportRequestObject> Handle(AddItemToQueueRequest request, CancellationToken cancellationToken)
    {
        if (request.PayLoad.Status == ExportingStatus.Failure)
            return request.PayLoad;

        var serviceBusSender = _serviceBusClient.CreateSender(_configuration[request.QueueType.ToString()]);

        request.PayLoad.Progress.Add(ExportingProgress.PlaceOnQueue);
        try
        {
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request.PayLoad)))
            {
                ContentType = "application/json",
                MessageId = request.PayLoad.Id.ToString()
            };

            await serviceBusSender.SendMessageAsync(message, cancellationToken);
        }
        catch (Exception)
        {
            request.PayLoad.Status = ExportingStatus.Failure;
            request.PayLoad.Progress.Add(ExportingProgress.FailToPlaceOnQueue);
        }

        return request.PayLoad;
    }
}