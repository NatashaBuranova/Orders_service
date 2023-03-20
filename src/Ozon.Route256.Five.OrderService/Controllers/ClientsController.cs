using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Clients;

namespace Ozon.Route256.Five.OrderService.Controllers;

public class ClientsController : BaseController
{
    public ClientsController() {}

    [HttpGet]
    public async Task<IActionResult> GetClientsListAsync()
    {
        await Task.Yield();
        return Ok(Enumerable.Empty<ClientResponse>());
    }
}