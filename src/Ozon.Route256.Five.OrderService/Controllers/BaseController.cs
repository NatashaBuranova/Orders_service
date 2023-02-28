using Microsoft.AspNetCore.Mvc;


namespace Ozon.Route256.Five.OrderService.Controllers;

[Area("Api")]
[ApiController]
[Route("[controller]/[action]")]
public abstract class BaseController : Controller
{
    protected BaseController() { }
}
