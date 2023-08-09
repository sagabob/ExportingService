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
}