using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.NotificationApi.Helpers.Core;
using ReportExporting.TestHelpers;
using SendGrid.Helpers.Mail;
using Xunit;

namespace ReportExporting.NotificationApiTests.Helpers;

public class EmailContentHelpersTests
{
    private const string FromEmail = "bobpham.tdp@gmail.com";
    private const string FromName = "Bob Pham";
    private const string AdminEmail = "bobpham.tdp@gmail.com";


    [Fact]
    public void CreateMessageForAdminTest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);


        var expectedMessage = new SendGridMessage
        {
            From = new EmailAddress(FromEmail, FromName),
            Subject = $"Notify on failure of report {reportRequestObject.FileName}",
            PlainTextContent = "Here is the order detail & error"
        };

        var helpers = new EmailContentHelpers();

        //Act
        var outputMsg = helpers.CreateMessageForAdmin(reportRequestObject, FromEmail, FromName);


        //Assert
        outputMsg.Should().BeEquivalentTo(expectedMessage);
    }

    [Fact]
    public void CreateMessageForAdminFromErrorMessageTest()
    {
        //Arrange
        var reportRequestErrorObjectFactory = new ReportRequestErrorObjectFactory();
        var reportRequestErrorObject = reportRequestErrorObjectFactory.CreateObjectErrorObject("fails sending email");


        //Expected
        var expectedMessage = new SendGridMessage
        {
            From = new EmailAddress(FromEmail, FromName),
            Subject = "Notify on failure",
            PlainTextContent = $"Here is the order detail & error: {reportRequestErrorObject.ErrorMassage}"
        };
        expectedMessage.AddTo(AdminEmail);


        var helpers = new EmailContentHelpers();

        //Act
        var outputMsg =
            helpers.CreateMessageForAdminFromErrorMessage(reportRequestErrorObject, FromEmail, FromName, AdminEmail);

        //Assert
        outputMsg.Should().BeEquivalentTo(expectedMessage);
    }

    [Fact]
    public void CreateMessageForClientTest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);


        var expectedMessage = new SendGridMessage
        {
            From = new EmailAddress(FromEmail, FromName),
            Subject = $"Your ordered report {reportRequestObject.FileName}",
            PlainTextContent = "Thank you for using our service, please see the attachment"
        };

        var helpers = new EmailContentHelpers();

        //Act
        var outputMsg = helpers.CreateMessageForClient(reportRequestObject, FromEmail, FromName);

        //Assert
        outputMsg.Should().BeEquivalentTo(expectedMessage);
    }

    [Fact]
    public void WrapReportRequestObjectToStreamTest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);


        var helpers = new EmailContentHelpers();

        var outputStream = helpers.WrapReportRequestObjectToStream(reportRequestObject);

        using var memoryStream = new MemoryStream();
        outputStream.CopyTo(memoryStream);
        var objectString = Encoding.UTF8.GetString(memoryStream.ToArray());

        var outputObject = JsonConvert.DeserializeObject<ReportRequestObject>(objectString);

        outputObject.Should().BeEquivalentTo(reportRequestObject);
    }

    [Fact]
    public async Task PrepareEmailContentForAdminTest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("123"));
        var helpers = new EmailContentHelpers();

        //Expected


        var output = await helpers.PrepareEmailContentForAdmin(reportRequestObject, fileStream, FromEmail, FromName,
            AdminEmail);

        output.Should().NotBeNull();
        output.From.Email.Should().Be(FromEmail);
        output.Attachments.Count.Should().Be(1);
    }

    [Fact]
    public async Task PrepareEmailContentForClientTest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        reportRequestObject.FileName = $"{reportRequestObject.Product.ToString()}-{reportRequestObject.Id}.json";

        var fileStream = new MemoryStream("123"u8.ToArray());
        var helpers = new EmailContentHelpers();

        //Expected


        var output = await helpers.PrepareEmailContentForClient(reportRequestObject, fileStream, FromEmail, FromName);
        
        output.Should().NotBeNull();
        output.From.Email.Should().Be(FromEmail);
        output.Attachments.Count.Should().Be(1);
    }
}