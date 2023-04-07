using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Core.Models.Enums;
using Ozon.Route256.Five.OrderService.Core.Services;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Services;
using Ozon.Route256.Five.OrderService.Web.Api.DTO.Orders;
using Ozon.Route256.Five.OrderService.Web.Api.Helpers;

namespace Ozon.Route256.Five.OrderService.Web.Api;

public class OrdersController : BaseController
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly ICancelOrderServices _cancelOrderServices;
    private readonly IClientRepository _clientRepository;
    private readonly IValidateOrderServices _validateOrderServices;


    public OrdersController(IOrderRepository orderRepository,
       ICancelOrderServices cancelOrderServices,
       IRegionRepository regionRepository,
       IClientRepository clientRepository,
       IValidateOrderServices validateOrderServices)
    {
        _orderRepository = orderRepository;
        _cancelOrderServices = cancelOrderServices;
        _regionRepository = regionRepository;
        _clientRepository = clientRepository;
        _validateOrderServices = validateOrderServices;
    }


    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrderByIdAsync(long id, CancellationToken token)
    {
        var order = await _orderRepository.FindAsync(id, token);

        if (order == null)
            return NotFound($"Order with id: {id} not found");

        if (_validateOrderServices.IsCanCancelOrder(order.State))
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

        var orders = await _orderRepository.GetOrdersListWithFiltersByPageAsync(
            OrderHelpers.ToOrdersListWithFiltersRepositoryRequest(request),
            token);

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

        var orders = await _orderRepository.GetOrdersForClientByTimePerPageAsync(
            OrderHelpers.ToOrdersForClientByTimeRepositoryRequest(request),
            token);

        return Ok(OrderHelpers.GetOrderResponse(orders));
    }
}