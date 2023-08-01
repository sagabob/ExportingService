using Microsoft.AspNetCore.Mvc;
using ReportExporting.ApplicationLib.Entities;
using ReportExporting.ApplicationLib.Helpers;
using ReportExporting.Core;
using ReportExporting.PlaceOrderApi.Handlers;
using ReportExporting.PlaceOrderApi.Messages;
using System.Diagnostics.CodeAnalysis;

namespace ReportExporting.PlaceOrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppTestingController : ControllerBase
{
    private readonly IExportRequestHandler _exportRequestHandler;
    private readonly IReportRequestObjectFactory _reportRequestObjectFactory;

    public AppTestingController(IExportRequestHandler exportRequestHandler,
        IReportRequestObjectFactory reportRequestObjectFactory)
    {
        _exportRequestHandler = exportRequestHandler;
        _reportRequestObjectFactory = reportRequestObjectFactory;
    }

    [ExcludeFromCodeCoverage]
    [HttpGet("TestPdfExport", Name = "TestPdfExport")]
    public async Task<ActionResult<ExportingReportResponse>> TestPdfExport()
    {
        var request = new ReportRequest
        {
            Title = "Sample Report",
            Product = ReportProduct.Profile,
            EmailAddress = "bobp@id.com.au",
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

        var result = await _exportRequestHandler.Handle(_reportRequestObjectFactory.CreateFromReportRequest(request));

        if (result.Status == ExportingStatus.Failure)
            return Forbid("Fail to process the order");

        var successResult = new ExportingReportResponse { OrderId = result.Id.ToString(), Status = "Order submitted" };

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
            EmailAddress = "bobp@id.com.au",
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

        var result = await _exportRequestHandler.Handle(_reportRequestObjectFactory.CreateFromReportRequest(request));

        if (result.Status == ExportingStatus.Failure)
            return Forbid("Fail to process the order");

        var successResult = new ExportingReportResponse { OrderId = result.Id.ToString(), Status = "Order submitted" };

        return Ok(successResult);
    }
}