using FluentAssertions;
using IronPdf;
using Microsoft.Extensions.Configuration;
using Moq;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.Core;
using ReportExporting.ExportApi.Generators.Core;
using ReportExporting.ExportApi.Helpers;
using ReportExporting.ExportApi.Helpers.Core;
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

        var exportConfigurationFactory = new ExportConfigurationFactory();

        var exportConfiguration = exportConfigurationFactory.GetConfiguration(requestObject);

        var exportObjectFactory = new ExportObjectFactory();

        var exportObject = exportObjectFactory.CreateExportObject(requestObject);

        //Mocking setup
        var pdfEngineWrapper = new Mock<IPdfEngineWrapper>();

        var pdfReportGenerator = new PdfReportGenerator(pdfEngineWrapper.Object);

        //Act
        var outputPdf =
            await pdfReportGenerator.RenderCoverPage(new ChromePdfRenderer(), exportConfiguration, exportObject)!;

        outputPdf.IsValid.Should().BeTrue();
    }

    [Theory(Skip = "Invalid license")]
    [InlineData(ReportProduct.Atlas)]
    [InlineData(ReportProduct.Economy)]
    public async Task GenerateReportAsyncTest(ReportProduct product)
    {
        //Arrange
        var request = TestDataFactory.GetFakeSingleReportRequest();
        request.Product = product;

        var reportRequestObjectFactory = new ReportRequestObjectFactory();

        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        var exportConfigurationFactory = new ExportConfigurationFactory();

        var exportConfiguration = exportConfigurationFactory.GetConfiguration(requestObject);

        var exportObjectFactory = new ExportObjectFactory();

        var exportObject = exportObjectFactory.CreateExportObject(requestObject);

        Mock<IConfiguration> configurationMock = new();
        var pdfEngineWrapper = new PdfEngineWrapper(configurationMock.Object);

        var pdfReportGenerator = new PdfReportGenerator(pdfEngineWrapper);

        //Act
        var outputPdfStream =
            await pdfReportGenerator.GenerateReportAsync(exportObject, exportConfiguration)!;

        outputPdfStream.Should().NotBeNull();
        outputPdfStream!.Length.Should().BeGreaterThan(0);
    }
}