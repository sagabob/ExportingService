using Microsoft.AspNetCore.Mvc;

namespace ReportExporting.ProcessOrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiInfoController : ControllerBase
{
    public IActionResult Info()
    {
        return Ok($"ProcessOrderApi-{DateTime.Today.ToLocalTime()}");
    }
}