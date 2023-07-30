using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;

namespace ReportExporting.NotificationApi.Handlers;

public class MessageForEmailHandler : IMessageForEmailHandler
{
    private readonly IConfiguration _configuration;
    private readonly ISendEmailHandler _sendEmailHandler;
    private readonly ServiceBusClient _serviceBusClient;
    private ServiceBusProcessor? _processor;

    public MessageForEmailHandler(ServiceBusClient serviceBusClient, IConfiguration configuration,
        ISendEmailHandler sendEmailHandler)
    {
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
        _sendEmailHandler = sendEmailHandler;
    }

    public async Task Register()
    {
        var options = new ServiceBusProcessorOptions
        {
            // By default after the message handler returns, the processor will complete the message
            // If I want more fine-grained control over settlement, I can set this to false.
            AutoCompleteMessages = false,

            // I can also allow for multi-threading
            MaxConcurrentCalls = 1
        };
        _processor = _serviceBusClient.CreateProcessor(_configuration["EmailQueue"], options);

        _processor.ProcessMessageAsync += ReceiveMessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync().ConfigureAwait(false);
        ;
    }

    private async Task ReceiveMessageHandler(ProcessMessageEventArgs args)
    {
        var blankReportRequestObject = new ReportRequestObject();
        try
        {
            var messageBody = Encoding.UTF8.GetString(args.Message.Body);
            var request = JsonConvert.DeserializeObject<ReportRequestObject>(messageBody);

            if (request != null)
            {
                if (request.Status != ExportingStatus.Failure)
                    await _sendEmailHandler.HandleSendingEmailToClient(request);
                else
                    // it means that the PlaceOrderApi sends this message to notify the admin
                    await _sendEmailHandler.HandleSendingEmailToAdmin(blankReportRequestObject);
            }
            else
            {
                blankReportRequestObject.ErrorMessage = "Fail to receive the request message in email queue";
                await _sendEmailHandler.HandleSendingEmailToAdmin(blankReportRequestObject);
            }
        }
        catch (Exception ex)
        {
            // ignored
            // will handle it later
            blankReportRequestObject.ErrorMessage = ex.Message;
            await _sendEmailHandler.HandleSendingEmailToAdmin(blankReportRequestObject);
        }
        finally
        {
            // we can evaluate application logic and use that to determine how to settle the message.
            await args.CompleteMessageAsync(args.Message);
        }
    }

    private async Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // the error source tells me at what point in the processing an error occurred
        Console.WriteLine(args.Exception.Message);

        var blankReportRequestObject = new ReportRequestObject { ErrorMessage = args.Exception.Message };

        await _sendEmailHandler.HandleSendingEmailToAdmin(blankReportRequestObject);
    }
}