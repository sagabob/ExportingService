using Microsoft.AspNetCore.Mvc;

namespace ReportExporting.NotificationApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiInfoController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ApiInfoController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Info()
    {
        var dockerImage = _configuration["DockerImage"];
        return Ok($"{dockerImage} - {DateTime.Today.ToLocalTime()}");
    }
}