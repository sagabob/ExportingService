using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Services.Core;

namespace ReportExporting.ApplicationLibTests;

public class TableStorageServiceTests
{
    public void UpsertEntityAsyncTest()
    {
        // Arrange

        var responseMock = new Mock<Response>();
        responseMock.SetupGet(r => r.Status).Returns(200);


        var tableServiceClientMock = new Mock<TableServiceClient>();
        Mock<IConfiguration> configuration = new();

        var tableClientMock = new Mock<TableClient>();

        tableServiceClientMock.Setup(x => x.GetTableClient(It.IsAny<string>())).Returns(tableClientMock.Object);

        tableClientMock.Setup(x => x.UpsertEntityAsync(It.IsAny<ReportRequestTableEntity>(), TableUpdateMode.Merge, default))
            .ReturnsAsync(responseMock.Object);


        var tableStorageService =
            new TableStorageService(tableServiceClientMock.Object, configuration.Object);

        tableStorageService.UpsertEntityAsync()

    }
}