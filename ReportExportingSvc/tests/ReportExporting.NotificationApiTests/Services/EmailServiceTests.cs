using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.NotificationApi.Helpers.Core;
using ReportExporting.NotificationApi.Services.Core;
using ReportExporting.TestHelpers;
using SendGrid;
using SendGrid.Helpers.Mail;
using Xunit;

namespace ReportExporting.NotificationApiTests.Services;

public class EmailServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock = new();

    public EmailServiceTests()
    {
        _configurationMock.SetupGet(x => x[It.Is<string>(s => s == "SendGridEmailSettings:FromName")])
            .Returns("Bob Pham");
        _configurationMock.SetupGet(x => x[It.Is<string>(s => s == "SendGridEmailSettings:FromEmail")])
            .Returns("bobp@id.com.au");
        _configurationMock.SetupGet(x => x[It.Is<string>(s => s == "SendGridEmailSettings:AdminEmail")])
            .Returns("bobp@id.com.au");
    }

    [Fact]
    public async Task SendingEmailToAdminAsyncTest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        Mock<ISendGridClient> sendGridClientMock = new();

        var mockResponse = new Response(HttpStatusCode.OK, null, null);


        sendGridClientMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
            .ReturnsAsync(mockResponse);

        var emailService =
            new EmailService(sendGridClientMock.Object, new EmailContentHelpers(), _configurationMock.Object);

        var output = await emailService.SendingEmailToAdminAsync(reportRequestObject);

        output.Progress.Contains(ExportingProgress.SendEmailToAdmin).Should().BeTrue();
    }

    [Fact]
    public async Task SendingEmailToAdminAsyncHandleException()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        Mock<ISendGridClient> sendGridClientMock = new();

        sendGridClientMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
            .ThrowsAsync(new Exception("Fails to send email"));

        var emailService =
            new EmailService(sendGridClientMock.Object, new EmailContentHelpers(), _configurationMock.Object);

        var output = await emailService.SendingEmailToAdminAsync(reportRequestObject);

        output.Progress.Contains(ExportingProgress.FailSendingEmailToAdmin).Should().BeTrue();
        output.ErrorMessage.Should().Be("Fails to send email");
    }

    [Fact]
    public async Task SendingEmailToAdminAsyncWhenFailingToSend()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        Mock<ISendGridClient> sendGridClientMock = new();

        var mockResponse = new Response(HttpStatusCode.BadRequest, null, null);


        sendGridClientMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
            .ReturnsAsync(mockResponse);

        var emailService =
            new EmailService(sendGridClientMock.Object, new EmailContentHelpers(), _configurationMock.Object);

        var output = await emailService.SendingEmailToAdminAsync(reportRequestObject);

        output.Progress.Contains(ExportingProgress.FailSendingEmailToAdmin).Should().BeTrue();
    }


    [Fact]
    public async Task SendingEmailToClientAsyncTest()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("123"));

        Mock<ISendGridClient> sendGridClientMock = new();

        var mockResponse = new Response(HttpStatusCode.OK, null, null);


        sendGridClientMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
            .ReturnsAsync(mockResponse);

        var emailService =
            new EmailService(sendGridClientMock.Object, new EmailContentHelpers(), _configurationMock.Object);

        var output = await emailService.SendingEmailToClientAsync(reportRequestObject, fileStream);

        output.Progress.Contains(ExportingProgress.SendEmailToClient).Should().BeTrue();
    }

    [Fact]
    public async Task SendingEmailToClientAsyncHandleException()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("123"));

        Mock<ISendGridClient> sendGridClientMock = new();


        sendGridClientMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
            .ThrowsAsync(new Exception("Fails to send email"));

        var emailService =
            new EmailService(sendGridClientMock.Object, new EmailContentHelpers(), _configurationMock.Object);

        var output = await emailService.SendingEmailToClientAsync(reportRequestObject, fileStream);

        output.Progress.Contains(ExportingProgress.FailSendingEmailToClient).Should().BeTrue();
        output.ErrorMessage.Should().Be("Fails to send email");
    }

    [Fact]
    public async Task SendingEmailToClientAsyncHandleFailureSendingEmail()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var reportRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("123"));

        Mock<ISendGridClient> sendGridClientMock = new();
        var mockResponse = new Response(HttpStatusCode.BadRequest, null, null);

        sendGridClientMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
            .ReturnsAsync(mockResponse);

        var emailService =
            new EmailService(sendGridClientMock.Object, new EmailContentHelpers(), _configurationMock.Object);

        var output = await emailService.SendingEmailToClientAsync(reportRequestObject, fileStream);

        output.Progress.Contains(ExportingProgress.FailSendingEmailToClient).Should().BeTrue();
        output.ErrorMessage.Should().Be(ExportingProgress.FailSendingEmailToClient.ToString());
    }

    [Fact]
    public async Task SendingEmailWithErrorToAdminAsyncTest()
    {
        var errorObjectFactory = new ReportRequestErrorObjectFactory();
        var errorObject = errorObjectFactory.CreateObjectErrorObject("Fail to Send");

        Mock<ISendGridClient> sendGridClientMock = new();

        var emailService =
            new EmailService(sendGridClientMock.Object, new EmailContentHelpers(), _configurationMock.Object);

        await emailService.SendingEmailWithErrorToAdminAsync(errorObject);

        sendGridClientMock.Verify(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), default), Times.Once);
    }
}