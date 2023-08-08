using FluentAssertions;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.ExportApi.Models;
using ReportExporting.ExportApi.Models.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ExportApiTests.Models;

public class ExportObjectFactoryTests
{
    [Fact]
    public void CreateExportObjectTest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObjectFactory = new ReportRequestObjectFactory();

        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        IExportObjectFactory factory = new ExportObjectFactory();

        var output = factory.CreateExportObject(requestObject);

        var expectedObject = new ExportObject
        {
            Urls = requestObject.Urls,
            Id = requestObject.Id.ToString(),
            Product = requestObject.Product,
            Format = requestObject.Format
        };

        output.Should().BeEquivalentTo(expectedObject);
    }
}