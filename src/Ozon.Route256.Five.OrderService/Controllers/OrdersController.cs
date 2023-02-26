using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

namespace Ozon.Route256.Five.OrderService.Controllers;

public class OrdersController : BaseController
{
    public OrdersController() {}

    [HttpGet]
    public async Task<IActionResult> CanceledOrderByIdAsync(long id)
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

    [HttpGet]
    public async Task<IActionResult> GetOrdersListWithFilters([FromQuery] OrdersListWithFiltersRequest request)
    {
        await Task.Yield();

        var orders = new List<OrderResponse>();
        return Ok(orders);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersListInRegionsByTime(DateTime startPeriod, List<long> regionIds)
    {
        await Task.Yield();

        var ordersinRegions = new List<OrdersInRegionResponse>();
        return Ok(ordersinRegions);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersForClientByTime([FromQuery] OrdersForClientByTimeRequest request)
    {
        await Task.Yield();

        var orders = new List<OrderResponse>();
        return Ok(orders);
    }
}