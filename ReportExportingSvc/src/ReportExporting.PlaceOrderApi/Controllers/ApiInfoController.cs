using Microsoft.AspNetCore.Mvc;

namespace ReportExporting.PlaceOrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiInfoController : ControllerBase
{
    public IActionResult Info()
    {
        return Ok($"PlaceOrderApi-{DateTime.Today.ToLocalTime()}");
    }
}