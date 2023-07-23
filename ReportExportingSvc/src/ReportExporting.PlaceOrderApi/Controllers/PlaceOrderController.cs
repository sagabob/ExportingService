using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using ReportExporting.Core;

namespace ReportExporting.PlaceOrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlaceOrderController : ControllerBase
{
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReportRequest>> PlaceExportOrder(ReportRequest request)
    {
        return Ok(request);
    }
}