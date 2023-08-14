using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.NotificationApi.Handlers;
using ReportExporting.NotificationApi.Handlers.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.NotificationApiTests.Handlers;

public class MessageForEmailHandlerTests
{
    [Fact]
    public async Task ReceiveMessageHandlerShouldProcessWhenGivenValidMessage()
    {
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        IReportRequestErrorObjectFactory errorObjectFactory = new ReportRequestErrorObjectFactory();

        var cts = new CancellationTokenSource();
        var serviceBusReceivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
            BinaryData.FromString(JsonConvert.SerializeObject(requestObject)),
            deliveryCount: 1);

        //Mock setup
        var serviceBusReceiverMock = new Mock<ServiceBusReceiver>();
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var sendEmailHandlerMock = new Mock<ISendEmailHandler>();
        Mock<IConfiguration> configurationMock = new();

        var messageForEmailHandler = new MessageForEmailHandler(serviceBusClientMock.Object,
            configurationMock.Object, sendEmailHandlerMock.Object, errorObjectFactory);

        var processMessageEventArgs =
            new ProcessMessageEventArgs(serviceBusReceivedMessage, serviceBusReceiverMock.Object, cts.Token);

        //Act
        await messageForEmailHandler.ReceiveMessageHandler(processMessageEventArgs);


        //Assert
        sendEmailHandlerMock.Verify(x => x.HandleSendingEmailToClient(It.IsAny<ReportRequestObject>()), Times.Once());
    }


    [Fact]
    public async Task ReceiveMessageHandlerShouldProcessWhenGivenFailureMessage()
    {
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.Status = ExportingStatus.Failure; //Message has failed

        IReportRequestErrorObjectFactory errorObjectFactory = new ReportRequestErrorObjectFactory();

        var cts = new CancellationTokenSource();
        var serviceBusReceivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
            BinaryData.FromString(JsonConvert.SerializeObject(requestObject))
        );

        //Mock setup
        var serviceBusReceiverMock = new Mock<ServiceBusReceiver>();
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var sendEmailHandlerMock = new Mock<ISendEmailHandler>();
        Mock<IConfiguration> configurationMock = new();

        var messageForEmailHandler = new MessageForEmailHandler(serviceBusClientMock.Object,
            configurationMock.Object, sendEmailHandlerMock.Object, errorObjectFactory);

        var processMessageEventArgs =
            new ProcessMessageEventArgs(serviceBusReceivedMessage, serviceBusReceiverMock.Object, cts.Token);

        //Act
        await messageForEmailHandler.ReceiveMessageHandler(processMessageEventArgs);


        //Assert
        sendEmailHandlerMock.Verify(x => x.HandleSendingEmailToAdmin(It.IsAny<ReportRequestObject>()), Times.Once());
    }

    [Fact]
    public async Task ReceiveMessageHandlerShouldProcessWhenGivenNullMessage()
    {
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.Status = ExportingStatus.Failure; //Message has failed

        IReportRequestErrorObjectFactory errorObjectFactory = new ReportRequestErrorObjectFactory();


        var cts = new CancellationTokenSource();
        var serviceBusReceivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(deliveryCount: 1);

        //Mock setup
        var serviceBusReceiverMock = new Mock<ServiceBusReceiver>();
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var sendEmailHandlerMock = new Mock<ISendEmailHandler>();
        Mock<IConfiguration> configurationMock = new();

        var messageForEmailHandler = new MessageForEmailHandler(serviceBusClientMock.Object,
            configurationMock.Object, sendEmailHandlerMock.Object, errorObjectFactory);

        var processMessageEventArgs =
            new ProcessMessageEventArgs(serviceBusReceivedMessage, serviceBusReceiverMock.Object, cts.Token);

        //Act
        await messageForEmailHandler.ReceiveMessageHandler(processMessageEventArgs);


        //Assert
        sendEmailHandlerMock.Verify(x => x.HandleSendingErrorEmailToAdmin(It.IsAny<ReportRequestErrorObject>()),
            Times.Once());
    }


    [Fact]
    public async Task ReceiveMessageHandlerShouldProcessWhenExceptionHappens()
    {
        IReportRequestErrorObjectFactory errorObjectFactory = new ReportRequestErrorObjectFactory();


        var cts = new CancellationTokenSource();
        var serviceBusReceivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage();

        //Mock setup
        var serviceBusReceiverMock = new Mock<ServiceBusReceiver>();
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var sendEmailHandlerMock = new Mock<ISendEmailHandler>();
        Mock<IConfiguration> configurationMock = new();

        sendEmailHandlerMock.Setup(x => x.HandleSendingEmailToClient(It.IsAny<ReportRequestObject>()))
            .ThrowsAsync(new Exception());

        var messageForEmailHandler = new MessageForEmailHandler(serviceBusClientMock.Object,
            configurationMock.Object, sendEmailHandlerMock.Object, errorObjectFactory);

        var processMessageEventArgs =
            new ProcessMessageEventArgs(serviceBusReceivedMessage, serviceBusReceiverMock.Object, cts.Token);

        //Act
        await messageForEmailHandler.ReceiveMessageHandler(processMessageEventArgs);


        //Assert
        sendEmailHandlerMock.Verify(x => x.HandleSendingErrorEmailToAdmin(It.IsAny<ReportRequestErrorObject>()),
            Times.Once());
    }

    [Fact]
    public async Task RegisterTest()
    {
        //Arrange
        IReportRequestErrorObjectFactory errorObjectFactory = new ReportRequestErrorObjectFactory();

        //Mock setup
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var sendEmailHandlerMock = new Mock<ISendEmailHandler>();
        Mock<IConfiguration> configurationMock = new();
        Mock<ServiceBusProcessor> serviceBusProcessorMock = new();

        serviceBusClientMock
            .Setup(x => x.CreateProcessor(It.IsAny<string>(), It.IsAny<ServiceBusProcessorOptions>()))
            .Returns(serviceBusProcessorMock.Object);

        var messageForEmailHandler = new MessageForEmailHandler(serviceBusClientMock.Object,
            configurationMock.Object, sendEmailHandlerMock.Object, errorObjectFactory);

        //Act
        await messageForEmailHandler.Register();


        //Assert
        serviceBusProcessorMock.Verify(x => x.StartProcessingAsync(default), Times.Once);
    }


    [Fact]
    public async Task ErrorHandlerTest()
    {
        //Arrange
        IReportRequestErrorObjectFactory errorObjectFactory = new ReportRequestErrorObjectFactory();

        //Mock setup
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var sendEmailHandlerMock = new Mock<ISendEmailHandler>();
        Mock<IConfiguration> configurationMock = new();


        var processErrorEventArgs =
            new ProcessErrorEventArgs(new Exception(), It.IsAny<ServiceBusErrorSource>(), It.IsAny<string>(),
                It.IsAny<string>(), default);

        var messageForEmailHandler = new MessageForEmailHandler(serviceBusClientMock.Object,
            configurationMock.Object, sendEmailHandlerMock.Object, errorObjectFactory);

        //Act
        await messageForEmailHandler.ErrorHandler(processErrorEventArgs);


        //Assert
        sendEmailHandlerMock.Verify(x => x.HandleSendingErrorEmailToAdmin(It.IsAny<ReportRequestErrorObject>()),
            Times.Once());
    }
}