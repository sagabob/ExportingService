using Azure;
using FluentAssertions;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.ApplicationLibTests.Entities;

public class ReportRequestTableEntityTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("bobpham.docx")]
    public void ObjectCreationTest(string filename)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObjectFactory = new ReportRequestObjectFactory();

        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.FileName = filename;

        var reportRequestTableEntityFactory = new ReportRequestTableEntityFactory();

        //expected object
        var reportRequestTableEntity = new ReportRequestTableEntity
        {
            FileName = requestObject.FileName,
            EmailAddress = requestObject.EmailAddress,
            PartitionKey = requestObject.Id,
            RowKey = requestObject.Id,
            Status = requestObject.Status.ToString(),
            FullProgress = requestObject.GetFullProgress()
        };

        //Act
        var tableEntity = reportRequestTableEntityFactory.CreateTableEntity(requestObject);

        //Assert
        tableEntity.Should().BeEquivalentTo(reportRequestTableEntity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("bobpham.docx")]
    public void ObjectFactorySkipTimestampAndDateTimeTest(string filename)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();

        var reportRequestObjectFactory = new ReportRequestObjectFactory();

        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.FileName = filename;

        var reportRequestTableEntityFactory = new ReportRequestTableEntityFactory();

        //expected object
        var reportRequestTableEntity = new ReportRequestTableEntity
        {
            FileName = requestObject.FileName,
            EmailAddress = requestObject.EmailAddress,
            PartitionKey = requestObject.Id,
            RowKey = requestObject.Id,
            Status = requestObject.Status.ToString(),
            FullProgress = requestObject.GetFullProgress(),
            Timestamp = DateTimeOffset.Now,
            ETag = new ETag("123")
        };

        //Act
        var tableEntity = reportRequestTableEntityFactory.CreateTableEntity(requestObject);

        //Assert
        tableEntity.Timestamp.Should().NotBe(reportRequestTableEntity.Timestamp);
        tableEntity.ETag.Should().NotBe(reportRequestTableEntity.ETag);
    }
}