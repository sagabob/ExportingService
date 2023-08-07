using Azure;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Handlers.Core;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ApplicationLib.Services;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ApplicationLibTests.Handlers;

public class UploadItemToBlobHandlerTests
{
    private readonly IReportRequestObjectFactory _reportRequestObjectFactory;

    public UploadItemToBlobHandlerTests()
    {
        _reportRequestObjectFactory = new ReportRequestObjectFactory();
    }

    [Fact]
    public async Task HandleShouldRejectFailureRequest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);
        reportRequestObject.Status = ExportingStatus.Failure;

        //Mocking setup
        var blobContent =
            BlobsModelFactory.BlobContentInfo(new ETag("123"), DateTimeOffset.Now, null, "123", 123);

        var response = Response.FromValue(blobContent, Mock.Of<Response>());

        var blobStorageServiceMock = new Mock<IBlobStorageService>();
        blobStorageServiceMock.Setup(x => x.UploadExportFileAync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(response);

        IUploadItemToBlobHandler uploadItemToBlobHandler = new UploadItemToBlobHandler(blobStorageServiceMock.Object);

        //Act
        var expectedResult = await uploadItemToBlobHandler.Handle(new MemoryStream(), reportRequestObject);

        //Assert
        expectedResult.Should().Be(reportRequestObject);

        //Verify function is not called
        blobStorageServiceMock.Verify(x => x.UploadExportFileAync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleShouldSuccessfullyProcessValidRequest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);


        //Mocking setup
        var blobContent =
            BlobsModelFactory.BlobContentInfo(new ETag("123"), DateTimeOffset.Now, null, "123", 123);

        var response = Response.FromValue(blobContent, Mock.Of<Response>());

        var blobStorageServiceMock = new Mock<IBlobStorageService>();
        blobStorageServiceMock.Setup(x => x.UploadExportFileAync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(response);

        IUploadItemToBlobHandler uploadItemToBlobHandler = new UploadItemToBlobHandler(blobStorageServiceMock.Object);

        //Act
        var expectedResult = await uploadItemToBlobHandler.Handle(new MemoryStream(), reportRequestObject);

        //Assert
        expectedResult.Progress.Contains(ExportingProgress.UploadFileToBlob).Should().BeTrue();
        expectedResult.Progress.Contains(ExportingProgress.FailUploadingFileToBlob).Should().BeFalse();

        //Verify function is called
        blobStorageServiceMock.Verify(x => x.UploadExportFileAync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
    }


    [Fact]
    public async Task HandleShouldManageExceptionWhenUploading()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);


        //Mocking setup
        var blobStorageServiceMock = new Mock<IBlobStorageService>();
        blobStorageServiceMock.Setup(x => x.UploadExportFileAync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Fail to upload"));

        IUploadItemToBlobHandler uploadItemToBlobHandler = new UploadItemToBlobHandler(blobStorageServiceMock.Object);

        //Act
        var expectedResult = await uploadItemToBlobHandler.Handle(new MemoryStream(), reportRequestObject);

        //Assert
        expectedResult.Progress.Contains(ExportingProgress.UploadFileToBlob).Should().BeTrue();
        expectedResult.Progress.Contains(ExportingProgress.FailUploadingFileToBlob).Should().BeTrue();

        //Verify function is called
        blobStorageServiceMock.Verify(x => x.UploadExportFileAync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
    }
}