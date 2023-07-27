using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Messages;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Messages;

namespace ReportExporting.PlaceOrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlaceOrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlaceOrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExportingReportResponse>> PlaceExportOrder(ReportRequest request)
    {
        var result = await _mediator.Send(new PlaceOrderRequest
            { PayLoad = ReportRequestObjectFactory.CreateFromReportRequest(request) });


        if (result.Status == ExportingStatus.Failure)
            return Forbid("Fail to process the order");

        var successResult = new ExportingReportResponse { OrderId = result.Id.ToString(), Status = "Order submitted" };

        return Ok(successResult);
    }

    [HttpGet("test", Name = "Test")]
    public async Task<IActionResult> Test()
    {
        var request = new ReportRequest
        {
            Title = "Sample Report",
            Product = ReportProduct.Profile,
            EmailAddress = "bobpham.tdp@gmail.com",
            Urls = new[]
            {
                new()
                {
                    Url = "https://profile.id.com.au/adelaide/ancestry",
                    Title = "Ancestry"
                },
                new ReportUrl
                {
                    Url = "https://profile.id.com.au/adelaide/industries",
                    Title = "Industries"
                }
            }
        };

        var result = await _mediator.Send(new PlaceOrderRequest { PayLoad = request });

        return Ok(result);
    }
}