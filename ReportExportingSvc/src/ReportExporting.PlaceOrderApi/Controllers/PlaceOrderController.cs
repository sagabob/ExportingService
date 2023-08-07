using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Handlers;
using ReportExporting.PlaceOrderApi.Messages;

namespace ReportExporting.PlaceOrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlaceOrderController : ControllerBase
{
    private readonly IExportRequestHandler _exportRequestHandler;
    private readonly IReportRequestObjectFactory _reportRequestObjectFactory;
    private readonly IValidator<ReportRequest> _reportValidator;


    public PlaceOrderController(IExportRequestHandler exportRequestHandler,
        IReportRequestObjectFactory reportRequestObjectFactory, IValidator<ReportRequest> reportValidator)
    {
        _exportRequestHandler = exportRequestHandler;
        _reportRequestObjectFactory = reportRequestObjectFactory;
        _reportValidator = reportValidator;
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExportingReportResponse>> PlaceExportOrder(ReportRequest request)
    {
        var validationResult = await _reportValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return BadRequest("Invalid report request");

        var requestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);

        var result = await _exportRequestHandler.Handle(requestObject);

        if (result.Status == ExportingStatus.Failure)
            return Forbid("Fail to process the order");

        var successResult = new ExportingReportResponse { OrderId = result.Id.ToString(), Status = "Order submitted" };

        return Ok(successResult);
    }
}