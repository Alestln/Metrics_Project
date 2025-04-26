using Microsoft.AspNetCore.Mvc;

namespace Metrics_Project.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        return Ok("Hello World!");
    }
}