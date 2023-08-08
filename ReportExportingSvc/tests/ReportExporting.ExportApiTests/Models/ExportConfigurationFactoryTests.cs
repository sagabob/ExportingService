using FluentAssertions;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.Core;
using ReportExporting.ExportApi.Models;
using ReportExporting.ExportApi.Models.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ExportApiTests.Models;

public class ExportConfigurationFactoryTests
{
    [Theory]
    [InlineData(ReportProduct.Economy, false)]
    [InlineData(ReportProduct.Atlas, true)]
    [InlineData(ReportProduct.Forecast, true)]
    [InlineData(ReportProduct.Housing, true)]
    [InlineData(ReportProduct.Profile, true)]
    public void GetConfigurationTest(ReportProduct product, bool showPageNumber)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObjectFactory = new ReportRequestObjectFactory();

        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        IExportConfigurationFactory exportConfigurationFactory = new ExportConfigurationFactory();

        requestObject.Product = product;
        var output = exportConfigurationFactory.GetConfiguration(requestObject);

        var expectedObject = new ExportConfiguration()
        {
            ShowPageNumber = showPageNumber,
            ShowCoverPage = true
        };

        output.Should().BeEquivalentTo(expectedObject);
    }
}