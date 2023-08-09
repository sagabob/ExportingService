using System.Text;
using FluentAssertions;
using Moq;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.Core;
using ReportExporting.ExportApi.Generators;
using ReportExporting.ExportApi.Generators.Core;
using ReportExporting.ExportApi.Models.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ExportApiTests.Generators;

public class ReportGeneratorFactoryTests
{
    [Theory]
    [InlineData(ReportFormat.Pdf)]
    [InlineData(ReportFormat.Word)]
    public async Task GenerateReportTest(ReportFormat format)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        request.Format = format;

        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        var exportConfigurationFactory = new ExportConfigurationFactory();
        var exportObjectFactory = new ExportObjectFactory();
        var pdfReportGeneratorMock = new Mock<IPdfReportGenerator>();
        var wordReportGeneratorMock = new Mock<IWordReportGenerator>();

        //Mocking setup
        var streamMock = new MemoryStream(Encoding.UTF8.GetBytes("123"));

        pdfReportGeneratorMock
            .Setup(x => x.GenerateReportAsync(It.IsAny<ExportObject>(), It.IsAny<ExportConfiguration>()))
            .ReturnsAsync(streamMock);

        wordReportGeneratorMock
            .Setup(x => x.GenerateReportAsync(It.IsAny<ExportObject>(), It.IsAny<ExportConfiguration>()))
            .ReturnsAsync(streamMock);

        var reportGeneratorFactory = new ReportGeneratorFactory(pdfReportGeneratorMock.Object,
            wordReportGeneratorMock.Object,
            exportConfigurationFactory, exportObjectFactory);

        //Act
        var outputStream = await reportGeneratorFactory.GenerateReport(requestObject);

        //Assert
        outputStream!.ToString().Should().Be(streamMock.ToString());

        //Verify
        if (format == ReportFormat.Pdf)
            pdfReportGeneratorMock.Verify(
                x => x.GenerateReportAsync(It.IsAny<ExportObject>(), It.IsAny<ExportConfiguration>()), Times.Once);
        else

            wordReportGeneratorMock.Verify(
                x => x.GenerateReportAsync(It.IsAny<ExportObject>(), It.IsAny<ExportConfiguration>()), Times.Once);
    }
}