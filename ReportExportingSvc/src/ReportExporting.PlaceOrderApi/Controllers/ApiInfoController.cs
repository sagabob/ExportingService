﻿using Microsoft.AspNetCore.Mvc;

namespace ReportExporting.PlaceOrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiInfoController(IConfiguration configuration) : ControllerBase
{
    public IActionResult Info()
    {
        var dockerImage = configuration["DockerImage"];
        return Ok($"{dockerImage} - {DateTime.Today.ToLocalTime()}");
    }
}