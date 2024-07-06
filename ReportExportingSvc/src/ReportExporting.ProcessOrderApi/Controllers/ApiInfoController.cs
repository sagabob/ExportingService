using Microsoft.AspNetCore.Mvc;

namespace ReportExporting.ProcessOrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiInfoController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("Info", Name = "Info")]
    public IActionResult Info()
    {
        var dockerImage = configuration["DockerImage"];
        return Ok($"{dockerImage} - {DateTime.Today.ToLocalTime()}");
    }
}