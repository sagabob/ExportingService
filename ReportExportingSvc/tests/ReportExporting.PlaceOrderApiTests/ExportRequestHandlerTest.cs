using FluentAssertions;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.PlaceOrderApi.Handlers;
using ReportExporting.PlaceOrderApi.Handlers.Core;
using ReportExporting.PlaceOrderApiTests.Helpers;
using Xunit;

namespace ReportExporting.PlaceOrderApiTests;

public class ExportRequestHandlerTests
{
    private readonly IExportRequestHandler _exportRequestHandler;

    private readonly Mock<IAddItemToQueueHandler> _addItemToQueueHandlerMock;

    private readonly Mock<IUpsertItemToTableHandler> _upsertItemToTableHandlerMock;
    public ExportRequestHandlerTests()
    {
        _addItemToQueueHandlerMock = new Mock<IAddItemToQueueHandler>();
        _upsertItemToTableHandlerMock = new Mock<IUpsertItemToTableHandler>();

        _exportRequestHandler =
            new ExportRequestHandler(_addItemToQueueHandlerMock.Object, _upsertItemToTableHandlerMock.Object);
    }

    [Fact]
    public async Task ShouldHandleExportRequest()
    {
        //Arrange
        var request = TestHelper.GetFakeReportRequest();
        var requestObject = ReportRequestObjectFactory.CreateFromReportRequest(request);

        requestObject.Status = ExportingStatus.Ongoing;

        _addItemToQueueHandlerMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.WorkQueue))
            .ReturnsAsync( () => requestObject);


        _upsertItemToTableHandlerMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(() => requestObject);

        //Act
        var updatedRequest = await _exportRequestHandler.Handle(requestObject);

        //Assert
        updatedRequest.Status.Should().Be(ExportingStatus.Ongoing);

        updatedRequest.Progress.Should().Contain(ExportingProgress.Submitting);

    }

    [Fact]
    public async Task ExportHandlerUseQueueAndTable()
    {
        // Arrange
        var request = TestHelper.GetFakeReportRequest();
        var requestObject = ReportRequestObjectFactory.CreateFromReportRequest(request);

        requestObject.Status = ExportingStatus.Ongoing;

        _addItemToQueueHandlerMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.WorkQueue))
            .ReturnsAsync(() => requestObject);


        _upsertItemToTableHandlerMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(() => requestObject);

        // Act
        await _exportRequestHandler.Handle(requestObject);

        // Assert
        _addItemToQueueHandlerMock.Verify(p => p.Handle(It.IsAny<ReportRequestObject>(), QueueType.WorkQueue), Times.Once());

        _upsertItemToTableHandlerMock.Verify(x => x.Handle(requestObject), Times.Exactly(2));
    }
}