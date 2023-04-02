using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Controllers;

public class ClientsController : BaseController
{
    private readonly IGetClientServices _clientServices;
    public ClientsController(IGetClientServices clientServices)
    {
        _clientServices = clientServices;
    }

    [HttpGet]
    public async Task<IActionResult> GetClientsListAsync(CancellationToken token)
    {
        var clients = await _clientServices.GetClientsAsync(token);

        return Ok(clients);
    }
}