using FluentAssertions;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ExportApi.Handlers;
using ReportExporting.ProcessOrderApi.Handlers.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ProcessOrderApiTests.Handlers;

public class HandleExportProcessTests
{
    [Fact]
    public async Task HandleShouldProcessAndUpdateValidRequest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObjectFactory = new ReportRequestObjectFactory();

        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        //Mock
        var addItemToQueueHandler = new Mock<IAddItemToQueueHandler>();
        var upsertItemToTableHandler = new Mock<IUpsertItemToTableHandler>();
        var exportRequestHandler = new Mock<IExportRequestHandler>();
        var uploadItemToBlobHandler = new Mock<IUploadItemToBlobHandler>();

        upsertItemToTableHandler.Setup(x => x.Handle(It.IsAny<ReportRequestObject>())).ReturnsAsync(requestObject);

        exportRequestHandler.Setup(x => x.ProcessExportRequest(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(new MemoryStream());

        addItemToQueueHandler.Setup(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.EmailQueue))
            .ReturnsAsync(requestObject);

        uploadItemToBlobHandler.Setup(x => x.Handle(It.IsAny<Stream>(), It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(requestObject);

        //Act
        var handleExportProcess = new HandleExportProcess(exportRequestHandler.Object, uploadItemToBlobHandler.Object,
            upsertItemToTableHandler.Object, addItemToQueueHandler.Object);

        var outputResult = await handleExportProcess.Handle(requestObject);

        //Assert
        outputResult.Status.Should().Be(ExportingStatus.Ongoing);
        outputResult.Progress.Contains(ExportingProgress.DoExportingOnOrder).Should().BeTrue();

        //Verify
        upsertItemToTableHandler.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Exactly(3));
        addItemToQueueHandler.Verify(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.EmailQueue), Times.Once);
        uploadItemToBlobHandler.Verify(x => x.Handle(It.IsAny<Stream>(), It.IsAny<ReportRequestObject>()), Times.Once);
        exportRequestHandler.Verify(x => x.ProcessExportRequest(It.IsAny<ReportRequestObject>()), Times.Once);
    }

    [Fact]
    public async Task HandleShouldManageExportingOutputNullStream()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObjectFactory = new ReportRequestObjectFactory();

        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        //Mock
        var addItemToQueueHandler = new Mock<IAddItemToQueueHandler>();
        var upsertItemToTableHandler = new Mock<IUpsertItemToTableHandler>();
        var exportRequestHandler = new Mock<IExportRequestHandler>();
        var uploadItemToBlobHandler = new Mock<IUploadItemToBlobHandler>();

        upsertItemToTableHandler.Setup(x => x.Handle(It.IsAny<ReportRequestObject>())).ReturnsAsync(requestObject);

        exportRequestHandler.Setup(x => x.ProcessExportRequest(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync((Stream?)null);

        addItemToQueueHandler.Setup(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.EmailQueue))
            .ReturnsAsync(requestObject);

        uploadItemToBlobHandler.Setup(x => x.Handle(It.IsAny<Stream>(), It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(requestObject);

        //Act
        var handleExportProcess = new HandleExportProcess(exportRequestHandler.Object, uploadItemToBlobHandler.Object,
            upsertItemToTableHandler.Object, addItemToQueueHandler.Object);

        var outputResult = await handleExportProcess.Handle(requestObject);

        //Assert
        outputResult.Status.Should().Be(ExportingStatus.Ongoing);
        outputResult.Progress.Contains(ExportingProgress.DoExportingOnOrder).Should().BeTrue();

        //Verify
        upsertItemToTableHandler.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Exactly(3));
        addItemToQueueHandler.Verify(x => x.Handle(It.IsAny<ReportRequestObject>(), QueueType.EmailQueue), Times.Once);
        uploadItemToBlobHandler.Verify(x => x.Handle(It.IsAny<Stream>(), It.IsAny<ReportRequestObject>()), Times.Never);
        exportRequestHandler.Verify(x => x.ProcessExportRequest(It.IsAny<ReportRequestObject>()), Times.Once);
    }
}