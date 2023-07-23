using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Controllers;
using Xunit;

namespace ReportExporting.PlaceOrderApiTests;

public class PlaceOrderControllerTest
{
    [Fact]
    public async Task CanPostExportRequestAsync()
    {
        //Arrange
        var request = Helper.GetFakeReportRequest();

        var placeOrderController = new PlaceOrderController();

        //Act
        var okResult = await placeOrderController.PlaceExportOrder(request);
        
        okResult.Should().NotBeNull();
        okResult?.Value.Should().Be(request);
        
    }
}