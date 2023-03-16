using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

namespace Ozon.Route256.Five.OrderService.Controllers;

public class OrdersController : BaseController
{
    public OrdersController() { }

    [HttpGet]
    public async Task<IActionResult> CancelOrderByIdAsync(long id)
    {
        await Task.Yield();
        return NotFound($"Order with id: {id} not found");
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderStatusByIdAsync(long id)
    {
        await Task.Yield();
        return NotFound($"Order with id: {id} not found");
    }

    [HttpPost]
    public async Task<IActionResult> GetOrdersListWithFiltersAsync([FromBody] OrdersListWithFiltersRequest request)
    {
        await Task.Yield();

        return Ok(Enumerable.Empty<OrderResponse>());
    }

    [HttpPost]
    public async Task<IActionResult> GetOrdersListByRegionsAndDateTimeAsync([FromBody] OrdersListByRegionsAndDateTimeRequest request)
    {
        await Task.Yield();

        return Ok(Enumerable.Empty<OrdersInRegionResponse>());
    }

    [HttpPost]
    public async Task<IActionResult> GetOrdersForClientByTimeAsync([FromBody] OrdersForClientByTimeRequest request)
    {
        await Task.Yield();

        return Ok(Enumerable.Empty<OrderResponse>());
    }
}