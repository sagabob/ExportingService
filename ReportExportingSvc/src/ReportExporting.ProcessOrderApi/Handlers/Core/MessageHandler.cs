using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Helpers;

namespace ReportExporting.ProcessOrderApi.Handlers.Core;

public class MessageHandler : IMessageHandler
{
    private readonly IAddErrorItemToQueueHandler _addErrorItemToQueueHandler;
    private readonly IAddItemToQueueHandler _addItemToQueueHandler;
    private readonly IConfiguration _configuration;
    private readonly IHandleExportProcess _handleExportProcess;
    private readonly IReportRequestErrorObjectFactory _reportRequestErrorObjectFactory;
    private readonly ServiceBusClient _serviceBusClient;
    private ServiceBusProcessor? _processor;

    public MessageHandler(ServiceBusClient serviceBusClient, IConfiguration configuration,
        IHandleExportProcess handleExportProcess, IAddItemToQueueHandler addItemToQueueHandler,
        IAddErrorItemToQueueHandler addErrorItemToQueueHandler,
        IReportRequestErrorObjectFactory reportRequestErrorObjectFactory)
    {
        _serviceBusClient = serviceBusClient;
        _configuration = configuration;
        _handleExportProcess = handleExportProcess;
        _addItemToQueueHandler = addItemToQueueHandler;
        _addErrorItemToQueueHandler = addErrorItemToQueueHandler;
        _reportRequestErrorObjectFactory = reportRequestErrorObjectFactory;
    }

    public async Task Register()
    {
        var options = new ServiceBusProcessorOptions
        {
            // By default after the message handler returns, the processor will complete the message
            // If I want more fine-grained control over settlement, I can set this to false.
            AutoCompleteMessages = false,

            // I can also allow for multi-threading
            MaxConcurrentCalls = 3
        };
        _processor = _serviceBusClient.CreateProcessor(_configuration["WorkQueue"], options);

        _processor.ProcessMessageAsync += ReceiveMessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync();
        ;
    }

    public async Task ReceiveMessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            var messageBody = Encoding.UTF8.GetString(args.Message.Body);
            var request = JsonConvert.DeserializeObject<ReportRequestObject>(messageBody);

            if (request != null)
            {
                request.Progress.Add(ExportingProgress.OrderReceivedFromQueue);
                await _handleExportProcess.Handle(request);
                ;
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
            var blankReportRequestObject = _reportRequestErrorObjectFactory.CreateObjectErrorObject(ex.Message);
            await _addErrorItemToQueueHandler.Handle(blankReportRequestObject, QueueType.EmailQueue);
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
        var blankReportRequestObject = _reportRequestErrorObjectFactory.CreateObjectErrorObject(args.Exception.Message);
        await _addErrorItemToQueueHandler.Handle(blankReportRequestObject, QueueType.EmailQueue);
    }
}