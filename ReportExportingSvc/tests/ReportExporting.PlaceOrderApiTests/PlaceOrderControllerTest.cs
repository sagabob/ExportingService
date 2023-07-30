using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.PlaceOrderApi.Controllers;
using ReportExporting.PlaceOrderApi.Handlers;
using Xunit;

namespace ReportExporting.PlaceOrderApiTests;

public class PlaceOrderControllerTest
{
    [Fact]
    public async Task CanPostExportRequestAsync()
    {
        //Arrange
        var request = Helper.GetFakeReportRequest();
        var requestObject = ReportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.Status = ExportingStatus.Ongoing;
        requestObject.Progress.Add(ExportingProgress.Submitting);


        var exportRequestHandlerMock = new Mock<IExportRequestHandler>();


        exportRequestHandlerMock.Setup(p => p.Handle(requestObject)).ReturnsAsync(requestObject);

        var placeOrderController = new PlaceOrderController(exportRequestHandlerMock.Object);

        //Act
        var actionResult = await placeOrderController.PlaceExportOrder(request);

        //Assert
        actionResult.Should().NotBeNull();

        var okResult = actionResult.Result as OkObjectResult;

        okResult?.Value.Should().NotBeNull();
    }
}