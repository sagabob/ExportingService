using Microsoft.AspNetCore.Mvc;

namespace ReportExporting.NotificationApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiInfoController : ControllerBase
{
    public IActionResult Info()
    {
        return Ok($"NotificationApi-{DateTime.Today.ToLocalTime()}");
    }
}