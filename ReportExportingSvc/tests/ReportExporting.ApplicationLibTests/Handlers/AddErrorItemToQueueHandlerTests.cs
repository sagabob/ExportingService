using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Handlers.Core;
using ReportExporting.ApplicationLib.Helpers.Core;
using Xunit;

namespace ReportExporting.ApplicationLibTests.Handlers;

public class AddErrorItemToQueueHandlerTests
{
    [Fact]
    public async Task HandleShouldProcessValidErrorMessage()
    {
        //Arrange
        var reportRequestErrorObjectFactory = new ReportRequestErrorObjectFactory();
        var reportRequestErrorObject = reportRequestErrorObjectFactory.CreateObjectErrorObject("123");

        //Mocking setup
        Mock<IConfiguration> configurationMock = new();

        var serviceBusClientMock = new Mock<ServiceBusClient>();

        var serviceBusSenderMock = new Mock<ServiceBusSender>();

        configurationMock.Setup(x => x[QueueType.EmailQueue.ToString()]).Returns(QueueType.EmailQueue.ToString());

        serviceBusClientMock.Setup(x => x.CreateSender(QueueType.EmailQueue.ToString()))
            .Returns(serviceBusSenderMock.Object);

        IAddErrorItemToQueueHandler handle =
            new AddErrorItemToQueueHandler(serviceBusClientMock.Object, configurationMock.Object);


        //Act
        var isThrown = await handle.Handle(reportRequestErrorObject, QueueType.EmailQueue);

        //Assert
        isThrown.Should().BeTrue();
        serviceBusSenderMock.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default), Times.Once);
    }


    [Fact]
    public async Task HandleShouldProcessWhenExceptionHappens()
    {
        //Arrange
        var reportRequestErrorObjectFactory = new ReportRequestErrorObjectFactory();
        var reportRequestErrorObject = reportRequestErrorObjectFactory.CreateObjectErrorObject("123");

        //Mocking setup
        Mock<IConfiguration> configurationMock = new();

        var serviceBusClientMock = new Mock<ServiceBusClient>();

        var serviceBusSenderMock = new Mock<ServiceBusSender>();

        configurationMock.Setup(x => x[QueueType.EmailQueue.ToString()]).Returns(QueueType.EmailQueue.ToString());

        serviceBusClientMock.Setup(x => x.CreateSender(QueueType.EmailQueue.ToString()))
            .Returns(serviceBusSenderMock.Object);

        //set up exception
        serviceBusSenderMock.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default))
            .ThrowsAsync(new ServiceBusException());

        IAddErrorItemToQueueHandler handle =
            new AddErrorItemToQueueHandler(serviceBusClientMock.Object, configurationMock.Object);


        //Act
        var isThrown = await handle.Handle(reportRequestErrorObject, QueueType.EmailQueue);

        //Assert
        isThrown.Should().BeFalse();
        serviceBusSenderMock.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default), Times.Once);
    }
}