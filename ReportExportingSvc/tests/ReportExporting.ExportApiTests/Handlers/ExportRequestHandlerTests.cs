using System.Text;
using FluentAssertions;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.Core;
using ReportExporting.ExportApi.Generators;
using ReportExporting.ExportApi.Handlers;
using ReportExporting.ExportApi.Handlers.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ExportApiTests.Handlers;

public class ExportRequestHandlerTests
{
    [Theory]
    [InlineData(ReportFormat.Pdf)]
    [InlineData(ReportFormat.Word)]
    public async Task ProcessExportRequestTest(ReportFormat reportFormat)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.Format = reportFormat;

        var expectedFileName =
            $"{requestObject.Product}-{requestObject.Id}.{(requestObject.Format == ReportFormat.Pdf ? "pdf" : "docx")}";

        //Mocking setup
        var reportedStream = new MemoryStream(Encoding.UTF8.GetBytes("123"));
        var reportGeneratorFactoryMock = new Mock<IReportGeneratorFactory>();
        reportGeneratorFactoryMock.Setup(x => x.GenerateReport(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(reportedStream);

        IExportRequestHandler exportRequestHandler = new ExportRequestHandler(reportGeneratorFactoryMock.Object);

        //Act
        var outputStream = await exportRequestHandler.ProcessExportRequest(requestObject);

        //Assert
        reportedStream.ToString().Should().BeEquivalentTo(outputStream!.ToString());

        requestObject.Progress
            .Contains(reportFormat == ReportFormat.Pdf ? ExportingProgress.ExportedPdf : ExportingProgress.ExportedWord)
            .Should().BeTrue();

        requestObject.FileName.Should().Be(expectedFileName);

        //Verify
        reportGeneratorFactoryMock.Verify(x => x.GenerateReport(It.IsAny<ReportRequestObject>()), Times.Once);
    }

    [Theory]
    [InlineData(ReportFormat.Pdf)]
    [InlineData(ReportFormat.Word)]
    public async Task ProcessExportRequestShouldHandleException(ReportFormat reportFormat)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.Format = reportFormat;

        //Mocking setup

        var reportGeneratorFactoryMock = new Mock<IReportGeneratorFactory>();
        reportGeneratorFactoryMock.Setup(x => x.GenerateReport(It.IsAny<ReportRequestObject>()))
            .ThrowsAsync(new Exception());

        IExportRequestHandler exportRequestHandler = new ExportRequestHandler(reportGeneratorFactoryMock.Object);

        //Act
        var outputStream = await exportRequestHandler.ProcessExportRequest(requestObject);

        //Assert
        outputStream.Should().BeNull();

        requestObject.Progress
            .Contains(reportFormat == ReportFormat.Pdf
                ? ExportingProgress.FailExportingPdf
                : ExportingProgress.FailExportingWord).Should().BeTrue();

        requestObject.FileName.Should().BeNull();

        requestObject.Status.Should().Be(ExportingStatus.Failure);

        //Verify
        reportGeneratorFactoryMock.Verify(x => x.GenerateReport(It.IsAny<ReportRequestObject>()), Times.Once);
    }
}