using Azure;
using Azure.Data.Tables;
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

public class UpsertItemToTableHandlerTests
{
    private readonly IReportRequestObjectFactory _reportRequestObjectFactory;
    private readonly IReportRequestTableEntityFactory _tableEntityFactory;

    public UpsertItemToTableHandlerTests()
    {
        _tableEntityFactory = new ReportRequestTableEntityFactory();
        _reportRequestObjectFactory = new ReportRequestObjectFactory();
    }

    [Theory]
    [InlineData(ExportingStatus.Failure)]
    [InlineData(ExportingStatus.Ongoing)]
    [InlineData(ExportingStatus.Success)]
    public async Task HandleShouldProcessRequestWithAllStatusAsync(ExportingStatus status)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);
        reportRequestObject.Status = status;

        //Mocking setup
        var responseMock = new Mock<Response>();
        responseMock.SetupGet(r => r.Status).Returns(204);
        var tableStorageServiceMock = new Mock<ITableStorageService>();
        tableStorageServiceMock.Setup(x => x.UpsertEntityAsync(It.IsAny<ReportRequestTableEntity>()))
            .ReturnsAsync(responseMock.Object);

        IUpsertItemToTableHandler upsertItemToTableHandler =
            new UpsertItemToTableHandler(tableStorageServiceMock.Object, _tableEntityFactory);

        //Act
        var output = await upsertItemToTableHandler.Handle(reportRequestObject);

        //Assert
        output.Progress.Contains(ExportingProgress.FailToUpsertToStore).Should().BeFalse();
        output.Status.Should().Be(status);

        //Verify function is called
        tableStorageServiceMock.Verify(x => x.UpsertEntityAsync(It.IsAny<ReportRequestTableEntity>()), Times.Once);
    }

    [Fact]
    public async Task HandleShouldUpdateRequestProgressWhenFailUpdating()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);


        //Mocking setup
        var responseMock = new Mock<Response>();
        responseMock.SetupGet(r => r.Status).Returns(400);
        var tableStorageServiceMock = new Mock<ITableStorageService>();
        tableStorageServiceMock.Setup(x => x.UpsertEntityAsync(It.IsAny<ReportRequestTableEntity>()))
            .ReturnsAsync(responseMock.Object);

        IUpsertItemToTableHandler upsertItemToTableHandler =
            new UpsertItemToTableHandler(tableStorageServiceMock.Object, _tableEntityFactory);

        //Act
        var output = await upsertItemToTableHandler.Handle(reportRequestObject);

        //Assert
        output.Progress.Contains(ExportingProgress.FailToUpsertToStore).Should().BeTrue();
        output.Status.Should().Be(ExportingStatus.Failure);

        //Verify function is called
        tableStorageServiceMock.Verify(x => x.UpsertEntityAsync(It.IsAny<ReportRequestTableEntity>()), Times.Once);
    }

    [Fact]
    public async Task HandleShouldUpdateRequestProgressWhenUpdatingThrowsException()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);


        //Mocking setup
        var responseMock = new Mock<Response>();
        responseMock.SetupGet(r => r.Status).Returns(400);
        var tableStorageServiceMock = new Mock<ITableStorageService>();

        tableStorageServiceMock.Setup(x => x.UpsertEntityAsync(It.IsAny<ReportRequestTableEntity>()))
            .ThrowsAsync(new TableTransactionFailedException(new RequestFailedException("Fail updating")));

        IUpsertItemToTableHandler upsertItemToTableHandler =
            new UpsertItemToTableHandler(tableStorageServiceMock.Object, _tableEntityFactory);

        //Act
        var output = await upsertItemToTableHandler.Handle(reportRequestObject);

        //Assert
        output.Progress.Contains(ExportingProgress.FailToUpsertToStore).Should().BeTrue();
        output.Status.Should().Be(ExportingStatus.Failure);

        //Verify function is called
        tableStorageServiceMock.Verify(x => x.UpsertEntityAsync(It.IsAny<ReportRequestTableEntity>()), Times.Once);
    }
}