using Azure;
using Azure.Data.Tables;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ApplicationLib.Services.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ApplicationLibTests.Services;

public class TableStorageServiceTests
{
    [Fact]
    public async Task UpsertEntityAsyncTest()
    {
        // Arrange

        var responseMock = new Mock<Response>();
        responseMock.SetupGet(r => r.Status).Returns(200);


        var tableServiceClientMock = new Mock<TableServiceClient>();
        Mock<IConfiguration> configuration = new();

        var tableClientMock = new Mock<TableClient>();

        tableServiceClientMock.Setup(x => x.GetTableClient(It.IsAny<string>())).Returns(tableClientMock.Object);

        tableClientMock.Setup(x =>
                x.UpsertEntityAsync(It.IsAny<ReportRequestTableEntity>(), TableUpdateMode.Merge, default))
            .ReturnsAsync(responseMock.Object);


        var tableStorageService =
            new TableStorageService(tableServiceClientMock.Object, configuration.Object);

        var request = TestDataFactory.GetFakeReportRequest();

        IReportRequestTableEntityFactory tableEntityFactory = new ReportRequestTableEntityFactory();
        IReportRequestObjectFactory reportRequestObjectFactory = new ReportRequestObjectFactory();

        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        var tableEntityObject = tableEntityFactory.CreateTableEntity(reportRequestObject);


        // Act
        var expectedResponse = await tableStorageService.UpsertEntityAsync(tableEntityObject);


        // Assertion
        expectedResponse.Status.Should().Be(200);
    }
}