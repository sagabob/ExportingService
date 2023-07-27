﻿using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using ReportExporting.Core;

namespace ReportExporting.ProcessOrderApi.Handlers;

public class MessageHandler
{
    private readonly ServiceBusProcessor _processor;

    public MessageHandler(ServiceBusClient serviceBusClient, IConfiguration configuration)
    {
        var options = new ServiceBusProcessorOptions
        {
            // By default after the message handler returns, the processor will complete the message
            // If I want more fine-grained control over settlement, I can set this to false.
            AutoCompleteMessages = false,

            // I can also allow for multi-threading
            MaxConcurrentCalls = 2
        };
        _processor = serviceBusClient.CreateProcessor(configuration["QueueName"], options);
    }

    public async Task Register()
    {
        _processor.ProcessMessageAsync += ReceiveMessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync();
    }

    private static async Task ReceiveMessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            var messageBody = Encoding.UTF8.GetString(args.Message.Body);
            var request = JsonConvert.DeserializeObject<ReportRequest>(messageBody);

            if (request != null)

        }
        catch (Exception e)
        {
        }
        finally
        {
            // we can evaluate application logic and use that to determine how to settle the message.
            await args.CompleteMessageAsync(args.Message);
        }
        

        
    }

    private static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // the error source tells me at what point in the processing an error occurred
        Console.WriteLine(args.ErrorSource);

        return Task.CompletedTask;
    }
}