using FluentAssertions;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.PlaceOrderApi.Handlers;
using ReportExporting.PlaceOrderApi.Handlers.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.PlaceOrderApiTests;

public class ExportRequestHandlerTests
{
    private readonly Mock<IAddItemToQueueHandler> _addItemToQueueHandlerMock;

    private readonly IReportRequestObjectFactory _reportRequestObjectFactory;

    private readonly Mock<IUpsertItemToTableHandler> _upsertItemToTableHandlerMock;

    private IExportRequestHandler _exportRequestHandler = null!;

    public ExportRequestHandlerTests()
    {
        _reportRequestObjectFactory = new ReportRequestObjectFactory();

        _addItemToQueueHandlerMock = new Mock<IAddItemToQueueHandler>();

        _upsertItemToTableHandlerMock = new Mock<IUpsertItemToTableHandler>();
    }

    [Fact]
    public async Task ShouldHandleExportRequest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var requestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);

        //Mocking setup
        _addItemToQueueHandlerMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.WorkQueue))
            .ReturnsAsync(() => requestObject);

        _upsertItemToTableHandlerMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(() => requestObject);

        _exportRequestHandler =
            new ExportRequestHandler(_addItemToQueueHandlerMock.Object, _upsertItemToTableHandlerMock.Object);

        //Act
        var updatedRequest = await _exportRequestHandler.Handle(requestObject);

        //Assert
        updatedRequest.Status.Should().Be(ExportingStatus.Ongoing);

        updatedRequest.Progress.Should().Contain(ExportingProgress.Submitting);

        //Verify functions called
        _addItemToQueueHandlerMock.Verify(p => p.Handle(It.IsAny<ReportRequestObject>(), QueueType.WorkQueue),
            Times.Once());

        _upsertItemToTableHandlerMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Exactly(2));
    }


    [Fact]
    public async Task ShouldHandleInvalidExportRequest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var requestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);

        //Create an expected failed object
        var expectedRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);
        expectedRequestObject.Status = ExportingStatus.Failure;


        //Mocking setup
        _addItemToQueueHandlerMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.WorkQueue))
            .ReturnsAsync(() => expectedRequestObject);

        _addItemToQueueHandlerMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.EmailQueue))
            .ReturnsAsync(() => expectedRequestObject);

        _upsertItemToTableHandlerMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(() => expectedRequestObject);

        _exportRequestHandler =
            new ExportRequestHandler(_addItemToQueueHandlerMock.Object, _upsertItemToTableHandlerMock.Object);

        //Act
        var updatedRequest = await _exportRequestHandler.Handle(requestObject);

        //Assert
        updatedRequest.Status.Should().Be(ExportingStatus.Failure);


        //Verify functions called
        _addItemToQueueHandlerMock.Verify(p => p.Handle(It.IsAny<ReportRequestObject>(), QueueType.WorkQueue),
            Times.Once());

        _upsertItemToTableHandlerMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Exactly(2));

        //Confirm that it is sent to the email queue by following the logic
        _addItemToQueueHandlerMock.Verify(p => p.Handle(It.IsAny<ReportRequestObject>(), QueueType.EmailQueue),
            Times.Once());
    }
}