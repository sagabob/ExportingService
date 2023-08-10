using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.ApplicationLib.Handlers.Core;

public class AddErrorItemToQueueHandler : IAddErrorItemToQueueHandler
{
    private readonly IConfiguration _configuration;
    private readonly ServiceBusClient _serviceBusClient;

    public AddErrorItemToQueueHandler(ServiceBusClient serviceBusClient, IConfiguration configuration)
    {
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
    }

    public async Task<bool> Handle(ReportRequestErrorObject requestErrorObject, QueueType queueType)
    {
        try
        {
            var serviceBusSender = _serviceBusClient.CreateSender(_configuration[queueType.ToString()]);

            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestErrorObject)))
            {
                ContentType = "application/json",
                MessageId = requestErrorObject.Id
            };

            await serviceBusSender.SendMessageAsync(message);
        }
        catch (Exception)
        {
            // log will be implemented here
            return false;
        }

        return true;
    }
}