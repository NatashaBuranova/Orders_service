using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Ozon.Route256.Five.OrderService.Controllers;

[Area("Api")]
[ApiController]
[Route("[controller]/[action]")]
public abstract class BaseController : Controller
{
    public static Logger Logger = LogManager.GetCurrentClassLogger();

    protected BaseController() { }
}
