using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Five.OrderService.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    public TestController(){}

    [HttpGet(Name = "test")]
    public IActionResult Get()
    {
        return Ok();
    }
}