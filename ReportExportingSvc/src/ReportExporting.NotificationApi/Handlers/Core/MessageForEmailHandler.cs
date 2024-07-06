using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;

namespace ReportExporting.NotificationApi.Handlers.Core;

public class MessageForEmailHandler : IMessageForEmailHandler
{
    private readonly IConfiguration _configuration;
    private readonly IReportRequestErrorObjectFactory _reportRequestErrorObjectFactory;
    private readonly ISendEmailHandler _sendEmailHandler;
    private readonly ServiceBusClient _serviceBusClient;
    private ServiceBusProcessor? _processor;

    public MessageForEmailHandler(ServiceBusClient serviceBusClient, IConfiguration configuration,
        ISendEmailHandler sendEmailHandler, IReportRequestErrorObjectFactory reportRequestErrorObjectFactory)
    {
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
        _sendEmailHandler = sendEmailHandler;
        _reportRequestErrorObjectFactory = reportRequestErrorObjectFactory;
    }

    public async Task Register()
    {
        var options = new ServiceBusProcessorOptions
        {
            // By default, after the message handler returns, the processor will complete the message
            // If I want more fine-grained control over settlement, I can set this to false.
            AutoCompleteMessages = false,

            // I can also allow for multi-threading
            MaxConcurrentCalls = 1
        };
        _processor = _serviceBusClient.CreateProcessor(_configuration["EmailQueue"], options);

        _processor.ProcessMessageAsync += ReceiveMessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync();
    }

    public async Task ReceiveMessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            var messageBody = Encoding.UTF8.GetString(args.Message.Body);
            var request = JsonConvert.DeserializeObject<ReportRequestObject>(messageBody);

            if (request != null)
            {
                request.Progress.Add(ExportingProgress.OrderReceivedFromEmailQueue);

                if (request.Status != ExportingStatus.Failure)
                    await _sendEmailHandler.HandleSendingEmailToClient(request);
                else
                    // it means that the PlaceOrderApi sends this message to notify the admin
                    await _sendEmailHandler.HandleSendingEmailToAdmin(request);
            }
            else
            {
                throw new Exception("Fail to parse the request from received message");
            }
        }
        catch (Exception ex)
        {
            // ignored
            // will handle it later

            await _sendEmailHandler.HandleSendingErrorEmailToAdmin(
                _reportRequestErrorObjectFactory.CreateObjectErrorObject(ex.Message));
        }
        finally
        {
            // we can evaluate application logic and use that to determine how to settle the message.
            await args.CompleteMessageAsync(args.Message);
        }
    }

    public async Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // the error source tells me at what point in the processing an error occurred

        await _sendEmailHandler.HandleSendingErrorEmailToAdmin(
            _reportRequestErrorObjectFactory.CreateObjectErrorObject(args.Exception.Message));
    }
}