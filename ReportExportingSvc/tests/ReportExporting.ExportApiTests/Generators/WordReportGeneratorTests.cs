using FluentAssertions;
using IronPdf;
using Microsoft.Extensions.Configuration;
using Moq;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ExportApi.Generators;
using ReportExporting.ExportApi.Generators.Core;
using ReportExporting.ExportApi.Helpers;
using ReportExporting.ExportApi.Helpers.Core;
using ReportExporting.ExportApi.Models.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ExportApiTests.Generators;

public class WordReportGeneratorTests
{
    [Fact(Skip = "Invalid license")]
    public async Task GenerateReportAsyncTest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeSingleReportRequest();

        var reportRequestObjectFactory = new ReportRequestObjectFactory();

        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        var exportConfigurationFactory = new ExportConfigurationFactory();

        var exportConfiguration = exportConfigurationFactory.GetConfiguration(requestObject);

        var exportObjectFactory = new ExportObjectFactory();

        var exportObject = exportObjectFactory.CreateExportObject(requestObject);

        //
        var configuration = new Mock<IConfiguration>();
        var pdfRenderer = new ChromePdfRenderer();
        var pdf = await pdfRenderer.RenderHtmlAsPdfAsync("<h1>Sample</h1>");

        var pdfGenerator = new Mock<IPdfReportGenerator>();
        pdfGenerator.Setup(x => x.GenerateReportAsync(It.IsAny<ExportObject>(), It.IsAny<ExportConfiguration>()))
            .ReturnsAsync(pdf.Stream);

        IWordEngineWrapper wordEngineWrapper = new WordEngineWrapper(configuration.Object);

        var wordReportGenerator = new WordReportGenerator(pdfGenerator.Object, wordEngineWrapper);

        //Act
        var docxStream = await wordReportGenerator.GenerateReportAsync(exportObject, exportConfiguration);

        //Assert
        docxStream.Should().NotBeNull();

        //TODO will look into how to unit test stream later
        docxStream!.Length.Should().BeGreaterThan(0);
    }
}