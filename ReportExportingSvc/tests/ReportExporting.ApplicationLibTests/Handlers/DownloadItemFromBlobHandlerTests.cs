using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ApplicationLib.Helpers;
using Xunit;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.TestHelpers;
using Azure;
using FluentAssertions;
using Moq;
using ReportExporting.ApplicationLib.Services;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Handlers.Core;

namespace ReportExporting.ApplicationLibTests.Handlers;

public class DownloadItemFromBlobHandlerTests
{
    private readonly IReportRequestObjectFactory _reportRequestObjectFactory;

    public DownloadItemFromBlobHandlerTests()
    {
        _reportRequestObjectFactory = new ReportRequestObjectFactory();
    }


    [Fact]
    public async Task HandleShouldRejectWhenFailureRequestGiven()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);
        reportRequestObject.Status = ExportingStatus.Failure;

        var blobStorageServiceMock = new Mock<IBlobStorageService>();


        IDownloadItemFromBlobHandler downloadItemFromBlobHandler =
            new DownloadItemFromBlobHandler(blobStorageServiceMock.Object);

        //Act
        var expectedResult = await downloadItemFromBlobHandler.Handle(new MemoryStream(), reportRequestObject);

        //Assert
        expectedResult.Progress.Contains(ExportingProgress.DownloadBlobToStream).Should().BeFalse();
        expectedResult.Status.Should().Be(ExportingStatus.Failure);

        //Verify
        blobStorageServiceMock.Verify(x => x.DownloadExportFileAync(It.IsAny<string>(), It.IsAny<Stream>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleShouldProcessSuccessfullyWhenValidRequestGiven()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);

        //Mocking setup
        var responseMock = new Mock<Response>();
        responseMock.SetupGet(r => r.Status).Returns(206);

        var blobStorageServiceMock = new Mock<IBlobStorageService>();
        blobStorageServiceMock.Setup(x => x.DownloadExportFileAync(It.IsAny<string>(),It.IsAny<Stream>()))
            .ReturnsAsync(responseMock.Object);

        IDownloadItemFromBlobHandler downloadItemFromBlobHandler =
            new DownloadItemFromBlobHandler(blobStorageServiceMock.Object);

        //Act
        var expectedResult = await downloadItemFromBlobHandler.Handle(new MemoryStream(), reportRequestObject);

        //Assert
        expectedResult.Progress.Contains(ExportingProgress.DownloadBlobToStream).Should().BeTrue();
        expectedResult.Progress.Contains(ExportingProgress.FailDownloadingBlobToStream).Should().BeFalse();

        //Verify
        blobStorageServiceMock.Verify(x => x.DownloadExportFileAync(It.IsAny<string>(), It.IsAny<Stream>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleShouldProcessUpdatingStatusProgressWhenFailingDownloadBlobStream()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);

        //Mocking setup
        var responseMock = new Mock<Response>();
        responseMock.SetupGet(r => r.Status).Returns(400); //Return with bad request code

        var blobStorageServiceMock = new Mock<IBlobStorageService>();
        blobStorageServiceMock.Setup(x => x.DownloadExportFileAync(It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(responseMock.Object);

        IDownloadItemFromBlobHandler downloadItemFromBlobHandler =
            new DownloadItemFromBlobHandler(blobStorageServiceMock.Object);

        //Act
        var expectedResult = await downloadItemFromBlobHandler.Handle(new MemoryStream(), reportRequestObject);

        //Assert
        expectedResult.Progress.Contains(ExportingProgress.DownloadBlobToStream).Should().BeTrue();
        expectedResult.Progress.Contains(ExportingProgress.FailDownloadingBlobToStream).Should().BeTrue();
        expectedResult.Status.Should().Be(ExportingStatus.Failure);

        //Verify
        blobStorageServiceMock.Verify(x => x.DownloadExportFileAync(It.IsAny<string>(), It.IsAny<Stream>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleShouldProcessUpdatingStatusProgressWhenDownloadingBlobStreamThrowsException()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);

        //Mocking setup
       
        var blobStorageServiceMock = new Mock<IBlobStorageService>();
        blobStorageServiceMock.Setup(x => x.DownloadExportFileAync(It.IsAny<string>(), It.IsAny<Stream>()))
            .ThrowsAsync(new Exception("All exceptions"));

        IDownloadItemFromBlobHandler downloadItemFromBlobHandler =
            new DownloadItemFromBlobHandler(blobStorageServiceMock.Object);

        //Act
        var expectedResult = await downloadItemFromBlobHandler.Handle(new MemoryStream(), reportRequestObject);

        //Assert
        expectedResult.Progress.Contains(ExportingProgress.DownloadBlobToStream).Should().BeTrue();
        expectedResult.Progress.Contains(ExportingProgress.FailDownloadingBlobToStream).Should().BeTrue();
        expectedResult.Status.Should().Be(ExportingStatus.Failure);

        //Verify
        blobStorageServiceMock.Verify(x => x.DownloadExportFileAync(It.IsAny<string>(), It.IsAny<Stream>()),
            Times.Once);
    }
}