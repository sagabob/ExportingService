using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ProcessOrderApi.Handlers;
using ReportExporting.ProcessOrderApi.Handlers.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ProcessOrderApiTests.Handlers;

public class MessageHandlerTests
{
    [Fact]
    public async Task ReceiveMessageHandlerShouldProcessSuccessfullyWithValidRequest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        var cts = new CancellationTokenSource();
        var serviceBusReceivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
            BinaryData.FromString(JsonConvert.SerializeObject(requestObject)),
            deliveryCount: 1);

        //Mock setup
        var serviceBusReceiverMock = new Mock<ServiceBusReceiver>();
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var addItemToQueueHandlerMock = new Mock<IAddItemToQueueHandler>();
        var handleExportProcessMock = new Mock<IHandleExportProcess>();
        Mock<IConfiguration> configurationMock = new();

        var msgHandler = new MessageHandler(serviceBusClientMock.Object, configurationMock.Object,
            handleExportProcessMock.Object, addItemToQueueHandlerMock.Object);

        var processMessageEventArgs =
            new ProcessMessageEventArgs(serviceBusReceivedMessage, serviceBusReceiverMock.Object, cts.Token);

        //Act
        await msgHandler.ReceiveMessageHandler(processMessageEventArgs);

        //Assert
        handleExportProcessMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Once);
    }


    [Fact]
    public async Task ReceiveMessageHandlerShouldSkipNextCallWhenNullRequestReceived()
    {
        //Arrange

        var cts = new CancellationTokenSource();

        //Null value will return when the object is parsed
        var serviceBusReceivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(deliveryCount: 1);

        //Mock setup
        var serviceBusReceiverMock = new Mock<ServiceBusReceiver>();
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var addItemToQueueHandlerMock = new Mock<IAddItemToQueueHandler>();
        var handleExportProcessMock = new Mock<IHandleExportProcess>();
        Mock<IConfiguration> configurationMock = new();

        var msgHandler = new MessageHandler(serviceBusClientMock.Object, configurationMock.Object,
            handleExportProcessMock.Object, addItemToQueueHandlerMock.Object);

        var processMessageEventArgs =
            new ProcessMessageEventArgs(serviceBusReceivedMessage, serviceBusReceiverMock.Object, cts.Token);

        //Act
        await msgHandler.ReceiveMessageHandler(processMessageEventArgs);

        //Assert
        handleExportProcessMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Never);
    }

    [Fact]
    public async Task ReceiveMessageHandlerShouldSkipNextCallWhenExceptionHappens()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        var cts = new CancellationTokenSource();
        var serviceBusReceivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
            BinaryData.FromString(JsonConvert.SerializeObject(requestObject)),
            deliveryCount: 1);

        //Mock setup
        var serviceBusReceiverMock = new Mock<ServiceBusReceiver>();
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var addItemToQueueHandlerMock = new Mock<IAddItemToQueueHandler>();
        var handleExportProcessMock = new Mock<IHandleExportProcess>();
        Mock<IConfiguration> configurationMock = new();

        handleExportProcessMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>())).ThrowsAsync(new Exception());

        var msgHandler = new MessageHandler(serviceBusClientMock.Object, configurationMock.Object,
            handleExportProcessMock.Object, addItemToQueueHandlerMock.Object);


        var processMessageEventArgs =
            new ProcessMessageEventArgs(serviceBusReceivedMessage, serviceBusReceiverMock.Object, cts.Token);

        //Act
        await msgHandler.ReceiveMessageHandler(processMessageEventArgs);

        //Assert
        handleExportProcessMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Once);
        addItemToQueueHandlerMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.EmailQueue),
            Times.Once);
    }

    [Fact]
    public async Task RegisterTest()
    {
        //Mock setup
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var addItemToQueueHandlerMock = new Mock<IAddItemToQueueHandler>();
        var handleExportProcessMock = new Mock<IHandleExportProcess>();
        Mock<IConfiguration> configurationMock = new();

        Mock<ServiceBusProcessor> serviceBusProcessorMock = new();

        serviceBusClientMock
            .Setup(x => x.CreateProcessor(It.IsAny<string>(), It.IsAny<ServiceBusProcessorOptions>()))
            .Returns(serviceBusProcessorMock.Object);

        var msgHandler = new MessageHandler(serviceBusClientMock.Object, configurationMock.Object,
            handleExportProcessMock.Object, addItemToQueueHandlerMock.Object);

        //Act
        await msgHandler.Register();

        //Verify
        serviceBusProcessorMock.Verify(x => x.StartProcessingAsync(default),Times.Once);
        
        //TODO Can't verify event handler is being added due to the function is sealed
        //serviceBusProcessorMock.VerifyAdd(x => x.ProcessMessageAsync += It.IsAny<Func<ProcessMessageEventArgs, Task>>());
    }

    [Fact]
    public async Task ErrorHandlerTest()
    {
        //Arrange
        //Mock setup
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var addItemToQueueHandlerMock = new Mock<IAddItemToQueueHandler>();
        var handleExportProcessMock = new Mock<IHandleExportProcess>();
        Mock<IConfiguration> configurationMock = new();

        var msgHandler = new MessageHandler(serviceBusClientMock.Object, configurationMock.Object,
            handleExportProcessMock.Object, addItemToQueueHandlerMock.Object);

        var processErrorEventArgs =
            new ProcessErrorEventArgs(new Exception(), It.IsAny<ServiceBusErrorSource>(), It.IsAny<string>(),
                It.IsAny<string>(), default);

        //Act
        await msgHandler.ErrorHandler(processErrorEventArgs);

        //Verify
        addItemToQueueHandlerMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.EmailQueue),
            Times.Once);

    }
}