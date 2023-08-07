using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Handlers.Core;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ApplicationLibTests.Handlers;

public class AddItemToQueueHandlerTests
{
    private readonly ReportRequestObject _reportRequestObject;

    public AddItemToQueueHandlerTests()
    {
        var request = TestDataFactory.GetFakeReportRequest();

        IReportRequestObjectFactory reportRequestObjectFactory = new ReportRequestObjectFactory();

        _reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
    }

    [Fact]
    public async Task HandleShouldRejectFailedRequestOnWorkQueue()
    {
        _reportRequestObject.Status = ExportingStatus.Failure;

        Mock<IConfiguration> configurationMock = new();
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        IAddItemToQueueHandler handle =
            new AddItemToQueueHandler(serviceBusClientMock.Object, configurationMock.Object);

        var expectedResult = await handle.Handle(_reportRequestObject, QueueType.WorkQueue);

        expectedResult.Should().Be(_reportRequestObject);
    }


    [Fact]
    public async Task HandleShouldAllowToSendFailedRequestOnEmailQueue()
    {
        // Arrange
        _reportRequestObject.Status = ExportingStatus.Failure;

        Mock<IConfiguration> configurationMock = new();
        var serviceBusClientMock = new Mock<ServiceBusClient>();

        var serviceBusSender = new Mock<ServiceBusSender>();
        serviceBusClientMock.Setup(x => x.CreateSender(QueueType.EmailQueue.ToString()))
            .Returns(serviceBusSender.Object);

        configurationMock.Setup(x => x[QueueType.EmailQueue.ToString()]).Returns(QueueType.EmailQueue.ToString());

        serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default)).Verifiable();

        IAddItemToQueueHandler handle =
            new AddItemToQueueHandler(serviceBusClientMock.Object, configurationMock.Object);


        var expectedResult = await handle.Handle(_reportRequestObject, QueueType.EmailQueue);

        expectedResult.Progress.Contains(ExportingProgress.SendOrderToEmailQueue).Should().BeTrue();

        serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default), Times.Once);
    }

    [Fact]
    public async Task HandleShouldAllowToSendValidRequestOnWorkQueue()
    {
        // Arrange
        _reportRequestObject.Status = ExportingStatus.Ongoing;

        Mock<IConfiguration> configurationMock = new();
        var serviceBusClientMock = new Mock<ServiceBusClient>();

        var serviceBusSender = new Mock<ServiceBusSender>();
        serviceBusClientMock.Setup(x => x.CreateSender(QueueType.WorkQueue.ToString()))
            .Returns(serviceBusSender.Object);

        configurationMock.Setup(x => x[QueueType.WorkQueue.ToString()]).Returns(QueueType.WorkQueue.ToString());

        serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default)).Verifiable();

        IAddItemToQueueHandler handle =
            new AddItemToQueueHandler(serviceBusClientMock.Object, configurationMock.Object);

        // Act
        var expectedResult = await handle.Handle(_reportRequestObject, QueueType.WorkQueue);


        // Assertion
        expectedResult.Progress.Contains(ExportingProgress.PlaceOrderOnQueue).Should().BeTrue();

        serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default), Times.Once);
    }

    [Fact]
    public async Task HandleShouldManageExceptionInSendingMessage()
    {
        // Arrange
        _reportRequestObject.Status = ExportingStatus.Ongoing;

        Mock<IConfiguration> configurationMock = new();
        var serviceBusClientMock = new Mock<ServiceBusClient>();

        var serviceBusSender = new Mock<ServiceBusSender>();
        serviceBusClientMock.Setup(x => x.CreateSender(QueueType.WorkQueue.ToString()))
            .Returns(serviceBusSender.Object);

        configurationMock.Setup(x => x[QueueType.WorkQueue.ToString()]).Returns(QueueType.WorkQueue.ToString());

        serviceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default))
            .ThrowsAsync(new ServiceBusException());

        IAddItemToQueueHandler handle =
            new AddItemToQueueHandler(serviceBusClientMock.Object, configurationMock.Object);

        // Act
        var expectedResult = await handle.Handle(_reportRequestObject, QueueType.WorkQueue);


        // Assertion
        expectedResult.Progress.Contains(ExportingProgress.PlaceOrderOnQueue).Should().BeTrue();
        expectedResult.Progress.Contains(ExportingProgress.FailToPlaceOrderOnQueue).Should().BeTrue();
        expectedResult.Status.Should().Be(ExportingStatus.Failure);

        serviceBusSender.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default), Times.Once);
    }
}