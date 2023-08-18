﻿using FluentAssertions;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Handlers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.NotificationApi.Handlers.Core;
using ReportExporting.NotificationApi.Services;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.NotificationApiTests.Handlers;

public class SendEmailHandlerTests
{
    [Fact]
    public async Task HandleSendingEmailToClientWhenProcessNotFailure()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);


        //Mocking setup
        var downloadItemFromBlobHandlerMock = new Mock<IDownloadItemFromBlobHandler>();
        var emailServiceMock = new Mock<IEmailService>();
        var upsertItemToTableHandlerMock = new Mock<IUpsertItemToTableHandler>();

        upsertItemToTableHandlerMock.Setup(x => x.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(requestObject);

        downloadItemFromBlobHandlerMock.Setup(x => x.Handle(It.IsAny<Stream>(), It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(requestObject);

        emailServiceMock.Setup(x => x.SendingEmailToClientAsync(It.IsAny<ReportRequestObject>(), It.IsAny<Stream>()))
            .ReturnsAsync(requestObject);

        var sendEmailHandler = new SendEmailHandler(emailServiceMock.Object, downloadItemFromBlobHandlerMock.Object,
            upsertItemToTableHandlerMock.Object);

        var outputResult = await sendEmailHandler.HandleSendingEmailToClient(requestObject);

        //Assert
        outputResult.Status.Should().Be(ExportingStatus.Success);

        //Verify
        downloadItemFromBlobHandlerMock.Verify(x => x.Handle(It.IsAny<Stream>(), It.IsAny<ReportRequestObject>()),
            Times.Once);
        
        emailServiceMock.Verify(x => x.SendingEmailToClientAsync(It.IsAny<ReportRequestObject>(), It.IsAny<Stream>()),
            Times.Once);

        emailServiceMock.Verify(x => x.SendingEmailToAdminAsync(It.IsAny<ReportRequestObject>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleSendingEmailToClientWhenProcessFailure()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        var failureRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        failureRequestObject.Status = ExportingStatus.Failure;


        //Mocking setup
        var downloadItemFromBlobHandlerMock = new Mock<IDownloadItemFromBlobHandler>();
        var emailServiceMock = new Mock<IEmailService>();
        var upsertItemToTableHandlerMock = new Mock<IUpsertItemToTableHandler>();

        upsertItemToTableHandlerMock.Setup(x => x.Handle(requestObject))
            .ReturnsAsync(requestObject);

        upsertItemToTableHandlerMock.Setup(x => x.Handle(failureRequestObject))
            .ReturnsAsync(failureRequestObject);

        //expect downloading handler fails
        downloadItemFromBlobHandlerMock.Setup(x => x.Handle(It.IsAny<Stream>(), It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(failureRequestObject);

        emailServiceMock.Setup(x => x.SendingEmailToAdminAsync(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(failureRequestObject);

        var sendEmailHandler = new SendEmailHandler(emailServiceMock.Object, downloadItemFromBlobHandlerMock.Object,
            upsertItemToTableHandlerMock.Object);

        var outputResult = await sendEmailHandler.HandleSendingEmailToClient(requestObject);

        //Assert
        outputResult.Status.Should().Be(ExportingStatus.Failure);

        //Verify
        downloadItemFromBlobHandlerMock.Verify(x => x.Handle(It.IsAny<Stream>(), It.IsAny<ReportRequestObject>()),
            Times.Once);
        
        emailServiceMock.Verify(x => x.SendingEmailToClientAsync(It.IsAny<ReportRequestObject>(), It.IsAny<Stream>()),
            Times.Never);

        emailServiceMock.Verify(x => x.SendingEmailToAdminAsync(It.IsAny<ReportRequestObject>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleSendingEmailToClientFails()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var reportRequestObjectFactory = new ReportRequestObjectFactory();
        var requestObject = reportRequestObjectFactory.CreateFromReportRequest(request);

        var failureRequestObject = reportRequestObjectFactory.CreateFromReportRequest(request);
        failureRequestObject.Status = ExportingStatus.Failure;

        //Mocking setup
        var downloadItemFromBlobHandlerMock = new Mock<IDownloadItemFromBlobHandler>();
        var emailServiceMock = new Mock<IEmailService>();
        var upsertItemToTableHandlerMock = new Mock<IUpsertItemToTableHandler>();

        upsertItemToTableHandlerMock.Setup(x => x.Handle(requestObject))
            .ReturnsAsync(requestObject);

        upsertItemToTableHandlerMock.Setup(x => x.Handle(failureRequestObject))
            .ReturnsAsync(failureRequestObject);

        downloadItemFromBlobHandlerMock.Setup(x => x.Handle(It.IsAny<Stream>(), It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(requestObject);

        emailServiceMock.Setup(x => x.SendingEmailToClientAsync(It.IsAny<ReportRequestObject>(), It.IsAny<Stream>()))
            .ReturnsAsync(failureRequestObject);

        emailServiceMock.Setup(x => x.SendingEmailToAdminAsync(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(failureRequestObject);

        var sendEmailHandler = new SendEmailHandler(emailServiceMock.Object, downloadItemFromBlobHandlerMock.Object,
            upsertItemToTableHandlerMock.Object);

        var outputResult = await sendEmailHandler.HandleSendingEmailToClient(requestObject);

        //Assert
        outputResult.Status.Should().Be(ExportingStatus.Failure);

        //Verify
        downloadItemFromBlobHandlerMock.Verify(x => x.Handle(It.IsAny<Stream>(), It.IsAny<ReportRequestObject>()),
            Times.Once);

        emailServiceMock.Verify(x => x.SendingEmailToClientAsync(It.IsAny<ReportRequestObject>(), It.IsAny<Stream>()),
            Times.Once);

        emailServiceMock.Verify(x => x.SendingEmailToAdminAsync(It.IsAny<ReportRequestObject>()),
            Times.Once);

    }
}