using FluentAssertions;
using IronPdf;
using Moq;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ExportApi.Generators.Core;
using ReportExporting.ExportApi.Helpers;
using ReportExporting.ExportApi.Models;
using ReportExporting.ExportApi.Models.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ExportApiTests.Generators;

public class PdfReportGeneratorTests
{
    [Fact]
    public async Task RenderCoverPageTest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObjectFactory = new ReportRequestObjectFactory();

        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        IExportConfigurationFactory exportConfigurationFactory = new ExportConfigurationFactory();

        var exportConfiguration = exportConfigurationFactory.GetConfiguration(requestObject);

        IExportObjectFactory exportObjectFactory = new ExportObjectFactory();

        var exportObject = exportObjectFactory.CreateExportObject(requestObject);


        var renderer = new ChromePdfRenderer();

        var samplePdf = await renderer.RenderHtmlAsPdfAsync($"<h1>{exportObject.Product.ToString()}</h1>");

        var pdfEngineWrapper = new Mock<IPdfEngineWrapper>();

        var pdfReportGenerator = new PdfReportGenerator(pdfEngineWrapper.Object);

        //Act
        var outputPdf = await pdfReportGenerator.RenderCoverPage(renderer, exportConfiguration, exportObject)!;

        outputPdf.BinaryData.Should().BeEquivalentTo(samplePdf.BinaryData);
    }
}