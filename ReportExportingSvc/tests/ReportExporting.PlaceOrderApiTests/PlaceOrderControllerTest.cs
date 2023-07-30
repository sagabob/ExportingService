using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.PlaceOrderApi.Controllers;
using ReportExporting.PlaceOrderApi.Handlers;
using ReportExporting.PlaceOrderApi.Messages;
using Xunit;

namespace ReportExporting.PlaceOrderApiTests;

public class PlaceOrderControllerTest
{
    [Theory]
    [InlineData(ExportingStatus.Ongoing)]
    [InlineData(ExportingStatus.Success)]
    public async Task CanGetValidSubmittedOrderWhenOrderStatusNotFailure(ExportingStatus status)
    {
        //Arrange
        var request = Helper.GetFakeReportRequest();
        var requestObject = ReportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.Status = status;


        var exportRequestHandlerMock = new Mock<IExportRequestHandler>();


        exportRequestHandlerMock.Setup(p => p.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(() => requestObject);

        var placeOrderController = new PlaceOrderController(exportRequestHandlerMock.Object);

        //Act
        var actionResult = await placeOrderController.PlaceExportOrder(request);

        //Assert
        actionResult.Should().NotBeNull();

        var okResult = actionResult.Result as OkObjectResult;

        okResult?.Value.Should().NotBeNull();

        var response = okResult!.Value as ExportingReportResponse;

        response!.OrderId.Should().Be(requestObject.Id.ToString());

        response!.Status.Should().Be("Order submitted");
    }

    [Fact]
    public async Task ReturnAFailedRequestWhenStatusFailure()
    {
        var request = Helper.GetFakeReportRequest();
        var requestObject = ReportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.Status = ExportingStatus.Failure;

        var exportRequestHandlerMock = new Mock<IExportRequestHandler>();


        exportRequestHandlerMock.Setup(p => p.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(() => requestObject);

        var placeOrderController = new PlaceOrderController(exportRequestHandlerMock.Object);

        //Act
        var actionResult = await placeOrderController.PlaceExportOrder(request);

        actionResult.Result.Should().NotBeNull();

        var outputMsg = actionResult.Result as ForbidResult;
        //Assert

        outputMsg.AuthenticationSchemes[0].Should().Be("Fail to process the order");
    }

}