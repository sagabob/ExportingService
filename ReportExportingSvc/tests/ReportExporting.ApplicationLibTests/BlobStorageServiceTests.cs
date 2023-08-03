using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ReportExporting.ApplicationLib.Services.Core;
using Xunit;

namespace ReportExporting.ApplicationLibTests;

public class BlobStorageServiceTests
{
    [Fact]
    public async Task UploadExportFileAyncTest()
    {
        // Arrange
        var blobContent =
            BlobsModelFactory.BlobContentInfo(new ETag("123"), DateTimeOffset.Now, null, "123", 123);

        var response = Response.FromValue(blobContent, Mock.Of<Response>());

        var blobServiceClient = new Mock<BlobServiceClient>();
        var blobContainerClient = new Mock<BlobContainerClient>();

        blobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClient.Object);

        blobContainerClient.Setup(x => x.UploadBlobAsync(It.IsAny<string>(), It.IsAny<MemoryStream>(), default))
            .ReturnsAsync(response);

        Mock<IConfiguration> configuration = new();

        var blobStorageService = new BlobStorageService(blobServiceClient.Object, configuration.Object);

        // Act
        var expectedResponse = await blobStorageService.UploadExportFileAync(new MemoryStream(), "123");


        // Assert
        expectedResponse.HasValue.Should().BeTrue();
    }

    [Fact]
    public async Task DownloadExportFileAyncTest()
    {
        // Arrange
        var responseMock = new Mock<Response>();
        responseMock.SetupGet(r => r.Status).Returns(206);

        var blobServiceClient = new Mock<BlobServiceClient>();
        var blobContainerClient = new Mock<BlobContainerClient>();
        var blobClient = new Mock<BlobClient>();

        blobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClient.Object);

        blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(blobClient.Object);

        blobClient.Setup(x => x.DownloadToAsync(It.IsAny<MemoryStream>())).ReturnsAsync(responseMock.Object);

        Mock<IConfiguration> configuration = new();

        var blobStorageService = new BlobStorageService(blobServiceClient.Object, configuration.Object);

        // Act
        var expectedResponse = await blobStorageService.DownloadExportFileAync("123", new MemoryStream());


        // Assert
        expectedResponse.Status.Should().Be(206);
    }
}