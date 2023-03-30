using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;
using Ozon.Route256.Five.OrderService.Controllers.Helpers;
using Ozon.Route256.Five.OrderService.Models.Enums;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Controllers;

public class OrdersController : BaseController
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly ICancelOrderServices _cancelOrderServices;
    private readonly IClientRepository _clientRepository;

    private static readonly HashSet<OrderState> ForbiddenToCancelStates = new()
    {
        OrderState.Cancelled,
        OrderState.Delivered
    };

    public OrdersController(IOrderRepository orderRepository,
       ICancelOrderServices cancelOrderServices,
       IRegionRepository regionRepository,
       IClientRepository clientRepository)
    {
        _orderRepository = orderRepository;
        _cancelOrderServices = cancelOrderServices;
        _regionRepository = regionRepository;
        _clientRepository = clientRepository;
    }


    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrderByIdAsync(long id, CancellationToken token)
    {       
        var order = await _orderRepository.FindAsync(id, token);

        if (order == null)
            return NotFound($"Order with id: {id} not found");

        if (ForbiddenToCancelStates.Contains(order.State))
            return BadRequest($"Cannot cancel order with id:{id} in state {order.State}");

        await _cancelOrderServices.CancelOrderInLogisticsSimulator(id, token);

        order.State = OrderState.Cancelled;
        await _orderRepository.UpdateAsync(order, token);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderStatusByIdAsync(long id, CancellationToken token)
    {
        var order = await _orderRepository.FindAsync(id, token);

        if (order == null)
            return NotFound($"Order with id: {id} not found");

        return Ok(order.State);
    }

    [HttpPost]
    public async Task<IActionResult> GetOrdersListWithFiltersAsync([FromBody] OrdersListWithFiltersRequest request, CancellationToken token)
    {
        foreach (var regionId in request.RegionFilterIds)
        {
            if (!await _regionRepository.IsExistsAsync(regionId, token))
                return NotFound($"Region with id: {regionId} not found");
        }

        var orders = await _orderRepository.GetOrdersListWithFiltersByPageAsync(request, token);

        return Ok(OrderHelpers.GetOrderResponse(orders));
    }

    [HttpPost]
    public async Task<IActionResult> GetOrdersListByRegionsAndDateTimeAsync([FromBody] OrdersListByRegionsAndDateTimeRequest request, CancellationToken token)
    {
        foreach (var regionId in request.regionIds)
        {
            if (!await _regionRepository.IsExistsAsync(regionId, token))
                return NotFound($"Region with id: {regionId} not found");
        }

        var orders = await _orderRepository.GetOrdersListByRegionsAndDateTimeAsync(request.startPeriod, request.regionIds, token);

        return Ok(OrderHelpers.GroupOrderByRegions(orders));
    }

    [HttpPost]
    public async Task<IActionResult> GetOrdersForClientByTimeAsync([FromBody] OrdersForClientByTimeRequest request, CancellationToken token)
    {
        if (!await _clientRepository.IsExistsAsync(request.ClientId, token))
            return NotFound($"Customer with id: {request.ClientId} not found");

        var orders = await _orderRepository.GetOrdersForClientByTimePerPageAsync(request, token);

        return Ok(OrderHelpers.GetOrderResponse(orders));
    }
}