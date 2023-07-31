using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.ApplicationLib.Helpers.Core;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Handlers;
using ReportExporting.PlaceOrderApi.Messages;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace ReportExporting.PlaceOrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlaceOrderController : ControllerBase
{
    private readonly IExportRequestHandler _exportRequestHandler;
    private readonly IReportRequestObjectFactory _reportRequestObjectFactory;
  

    public PlaceOrderController(IExportRequestHandler exportRequestHandler, IReportRequestObjectFactory reportRequestObjectFactory)
    {
        _exportRequestHandler = exportRequestHandler;
        _reportRequestObjectFactory = reportRequestObjectFactory;
        
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExportingReportResponse>> PlaceExportOrder(ReportRequest request)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ModelState);
        }

        var requestObject = _reportRequestObjectFactory.CreateFromReportRequest(request);

        var result = await _exportRequestHandler.Handle(requestObject);

        if (result.Status == ExportingStatus.Failure)
            return Forbid("Fail to process the order");

        var successResult = new ExportingReportResponse { OrderId = result.Id.ToString(), Status = "Order submitted" };

        return Ok(successResult);
    }

  
}