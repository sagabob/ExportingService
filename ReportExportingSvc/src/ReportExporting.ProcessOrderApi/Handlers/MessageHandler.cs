using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;

namespace ReportExporting.ProcessOrderApi.Handlers;

public class MessageHandler : IMessageHandler
{
    private readonly IAddItemToQueueHandler _addItemToQueueHandler;
    private readonly IConfiguration _configuration;
    private readonly IHandleExportProcess _handleExportProcess;
    private readonly ServiceBusClient _serviceBusClient;
    private ServiceBusProcessor? _processor;

    public MessageHandler(ServiceBusClient serviceBusClient, IConfiguration configuration,
        IHandleExportProcess handleExportProcess, IAddItemToQueueHandler addItemToQueueHandler)
    {
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
        _handleExportProcess = handleExportProcess;
        _addItemToQueueHandler = addItemToQueueHandler;
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
        _processor = _serviceBusClient.CreateProcessor(_configuration["WorkQueue"], options);

        _processor.ProcessMessageAsync += ReceiveMessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync().ConfigureAwait(false);
        ;
    }

    private async Task ReceiveMessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            var messageBody = Encoding.UTF8.GetString(args.Message.Body);
            var request = JsonConvert.DeserializeObject<ReportRequestObject>(messageBody);

            if (request != null)
            {
                request.Progress.Add(ExportingProgress.OrderReceivedFromQueue);
                await _handleExportProcess.Handle(request).ConfigureAwait(false);
                ;
            }
        }
        catch (Exception ex)
        {
            // ignored
            // will handle it later
            var blankReportRequestObject = new ReportRequestObject { ErrorMessage = ex.Message };
            await _addItemToQueueHandler.Handle(blankReportRequestObject, QueueType.EmailQueue);
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
        Console.WriteLine(args.ErrorSource);
        var blankReportRequestObject = new ReportRequestObject { ErrorMessage = args.Exception.Message };
        await _addItemToQueueHandler.Handle(blankReportRequestObject, QueueType.EmailQueue);
    }
}