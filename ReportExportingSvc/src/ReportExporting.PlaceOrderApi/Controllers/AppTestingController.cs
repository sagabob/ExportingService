﻿using System.Diagnostics.CodeAnalysis;
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
public class AppTestingController(
    IExportRequestHandler exportRequestHandler,
    IReportRequestObjectFactory reportRequestObjectFactory,
    IValidator<ReportRequest> reportValidator)
    : ControllerBase
{
    [ExcludeFromCodeCoverage]
    [HttpGet("TestPdfExport", Name = "TestPdfExport")]
    public async Task<ActionResult<ExportingReportResponse>> TestPdfExport()
    {
        var request = new ReportRequest
        {
            Title = "Sample Report",
            Product = ReportProduct.Profile,
            EmailAddress = "bobpham.tdp@live.com",
            Format = ReportFormat.Pdf,
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

        var validationResult = await reportValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return BadRequest("Invalid report request");

        var result = await exportRequestHandler.Handle(reportRequestObjectFactory.CreateFromReportRequest(request));

        if (result.Status == ExportingStatus.Failure)
            return Forbid("Fail to process the order");

        var successResult = new ExportingReportResponse { OrderId = result.Id, Status = "Order submitted" };

        return Ok(successResult);
    }

    [ExcludeFromCodeCoverage]
    [HttpGet("TestWordExport", Name = "TestWordExport")]
    public async Task<ActionResult<ExportingReportResponse>> TestWordExport()
    {
        var request = new ReportRequest
        {
            Title = "Sample Report",
            Product = ReportProduct.Profile,
            EmailAddress = "bobpham.tdp@live.com",
            Format = ReportFormat.Word,
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

        var validationResult = await reportValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return BadRequest("Invalid report request");

        var result = await exportRequestHandler.Handle(reportRequestObjectFactory.CreateFromReportRequest(request));

        if (result.Status == ExportingStatus.Failure)
            return Forbid("Fail to process the order");

        var successResult = new ExportingReportResponse { OrderId = result.Id, Status = "Order submitted" };

        return Ok(successResult);
    }
}