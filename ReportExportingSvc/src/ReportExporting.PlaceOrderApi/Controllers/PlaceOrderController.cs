using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Requests;

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
    public async Task<ActionResult<ReportRequest>> PlaceExportOrder(ReportRequest request)
    {
        var result = await _mediator.Send(new PlaceOrderRequest { PayLoad = request });
        return Ok(result);
    }

    [HttpGet("test", Name = "Test")]
    public async Task<IActionResult> Test()
    {
        var request = new ReportRequest
        {
            Title = "Sample Report",
            Product = ReportProduct.Profile,
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