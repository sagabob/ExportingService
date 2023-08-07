using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Controllers;
using ReportExporting.PlaceOrderApi.Handlers;
using ReportExporting.PlaceOrderApi.Messages;
using ReportExporting.TestHelpers;
using Xunit;

namespace ReportExporting.PlaceOrderApiTests;

public class PlaceOrderControllerTests
{
    private readonly IReportRequestObjectFactory _reportRequestObjectFactory;

    public PlaceOrderControllerTests()
    {
        _reportRequestObjectFactory = new ReportRequestObjectFactory();
    }

    [Theory]
    [InlineData(ExportingStatus.Ongoing)]
    [InlineData(ExportingStatus.Success)]
    public async Task CanGetValidSubmittedOrderWhenOrderStatusNotFailure(ExportingStatus status)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var requestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.Status = status;

        //Mocking setup
        var exportRequestHandlerMock = new Mock<IExportRequestHandler>();

        exportRequestHandlerMock.Setup(p => p.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(() => requestObject);

        var placeOrderController = new PlaceOrderController(exportRequestHandlerMock.Object,
            _reportRequestObjectFactory, new ExportRequestValidator());

        //Act
        var actionResult = await placeOrderController.PlaceExportOrder(request);

        //Assert
        actionResult.Should().NotBeNull();

        var okResult = actionResult.Result as OkObjectResult;

        okResult?.Value.Should().NotBeNull();

        var response = okResult!.Value as ExportingReportResponse;

        response!.OrderId.Should().Be(requestObject.Id.ToString());

        response!.Status.Should().Be("Order submitted");

        //Confirm the handler is called
        exportRequestHandlerMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Once);
    }

    [Fact]
    public async Task ReturnFailedRequestWhenStatusFailure()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        var requestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);
        requestObject.Status = ExportingStatus.Failure;


        //Mocking setup
        var exportRequestHandlerMock = new Mock<IExportRequestHandler>();

        exportRequestHandlerMock.Setup(p => p.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(() => requestObject);

        var placeOrderController = new PlaceOrderController(exportRequestHandlerMock.Object,
            _reportRequestObjectFactory, new ExportRequestValidator());

        //Act
        var actionResult = await placeOrderController.PlaceExportOrder(request);


        //Assert
        actionResult.Result.Should().NotBeNull();

        var outputMsg = actionResult.Result as ForbidResult;

        outputMsg!.AuthenticationSchemes[0].Should().Be("Fail to process the order");

        //Confirm the handler is called
        exportRequestHandlerMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Once);
    }


    [Fact]
    public async Task ReturnBadRequestWhenRequestHasNoUrls()
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        request.Urls = null!;

        //Mocking setup
        var exportRequestHandlerMock = new Mock<IExportRequestHandler>();

        var placeOrderController = new PlaceOrderController(exportRequestHandlerMock.Object,
            _reportRequestObjectFactory, new ExportRequestValidator());


        //Act
        var actionResult = await placeOrderController.PlaceExportOrder(request);


        //Assertion
        actionResult.Result.Should().NotBeNull();

        var outputMsg = actionResult.Result as BadRequestObjectResult;

        outputMsg!.StatusCode.Should().Be(400);
        outputMsg.Value.Should().Be("Invalid report request");
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("bob")]
    public async Task ReturnBadRequestWhenRequestHasInvalidUrls(string inputUrl)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        request.Urls = null!;

        request.Urls = new[]
        {
            new()
            {
                Url = "https://profile.id.com.au/adelaide/ancestry",
                Title = "Ancestry"
            },
            new ReportUrl
            {
                Url = inputUrl,
                Title = "Industries"
            }
        };

        //Mocking setup
        var exportRequestHandlerMock = new Mock<IExportRequestHandler>();

        var placeOrderController = new PlaceOrderController(exportRequestHandlerMock.Object,
            _reportRequestObjectFactory, new ExportRequestValidator());

        //Act
        var actionResult = await placeOrderController.PlaceExportOrder(request);


        //Assertion
        actionResult.Result.Should().NotBeNull();

        var outputMsg = actionResult.Result as BadRequestObjectResult;

        outputMsg!.StatusCode.Should().Be(400);
        outputMsg.Value.Should().Be("Invalid report request");
    }


    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("bobpham", false)]
    [InlineData("bobpham#@gmail.com", true)]
    [InlineData("bobpham.tdp@gmail.com", true)]
    [InlineData("bobpham-tdp@gmail.com", true)]
    [InlineData("bobpham_tdp@gmail.com", true)]
    [InlineData("bobpham_tdp@gmail.&com", false)]
    public async Task ReturnBadRequestWhenRequestHasInvalidEmailAddress(string emailAddress, bool expectedResult)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        request.EmailAddress = emailAddress;

        var requestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);


        //Mocking setup
        var exportRequestHandlerMock = new Mock<IExportRequestHandler>();


        exportRequestHandlerMock.Setup(p => p.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(() => requestObject);

        var placeOrderController = new PlaceOrderController(exportRequestHandlerMock.Object,
            _reportRequestObjectFactory, new ExportRequestValidator());

        //Act
        var actionResult = await placeOrderController.PlaceExportOrder(request);

        //Assertion
        if (!expectedResult)
        {
            var outputMsg = actionResult.Result as BadRequestObjectResult;

            outputMsg!.StatusCode.Should().Be(400);
            outputMsg.Value.Should().Be("Invalid report request");


            //Verify function is not called
            exportRequestHandlerMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Never);
        }
        else
        {
            var okResult = actionResult.Result as OkObjectResult;

            okResult?.Value.Should().NotBeNull();

            var response = okResult!.Value as ExportingReportResponse;

            response!.OrderId.Should().Be(requestObject.Id.ToString());

            response!.Status.Should().Be("Order submitted");

            //Verify function is called
            exportRequestHandlerMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Once);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("population estimate", true)]
    public async Task ReturnBadRequestWhenRequestHasInvalidTitle(string title, bool result)
    {
        //Arrange
        var request = TestDataFactory.GetFakeReportRequest();
        request.Title = title;

        var requestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);

        //Mocking setup
        var exportRequestHandlerMock = new Mock<IExportRequestHandler>();

        exportRequestHandlerMock.Setup(p => p.Handle(It.IsAny<ReportRequestObject>()))
            .ReturnsAsync(() => requestObject);

        var placeOrderController = new PlaceOrderController(exportRequestHandlerMock.Object,
            _reportRequestObjectFactory, new ExportRequestValidator());

        
        //Act
        var actionResult = await placeOrderController.PlaceExportOrder(request);


        //Assertion
        if (!result)
        {
            var outputMsg = actionResult.Result as BadRequestObjectResult;

            outputMsg!.StatusCode.Should().Be(400);
            outputMsg.Value.Should().Be("Invalid report request");

            //Verify function is not called
            exportRequestHandlerMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Never);
        }
        else
        {
            var okResult = actionResult.Result as OkObjectResult;

            okResult?.Value.Should().NotBeNull();

            var response = okResult!.Value as ExportingReportResponse;

            response!.OrderId.Should().Be(requestObject.Id.ToString());

            response!.Status.Should().Be("Order submitted");

            //Verify function is called
            exportRequestHandlerMock.Verify(x => x.Handle(It.IsAny<ReportRequestObject>()), Times.Once);
        }
    }
}