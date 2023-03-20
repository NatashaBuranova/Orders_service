using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;
using Ozon.Route256.Five.OrderService.Models.Enums;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Controllers;

public class OrdersController : BaseController
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly ICanceledOrderServices _canceledOrderServices;
    private readonly IGetClientServices _clientServices;

    private static readonly HashSet<OrderState> ForbiddenToCancelStates = new()
    {
        OrderState.Cancelled,
        OrderState.Delivered
    };

    public OrdersController(IOrderRepository orderRepository,
        ICanceledOrderServices canceledOrderServices,
        IRegionRepository regionRepository,
        IGetClientServices clientServices)
    {
        _orderRepository = orderRepository;
        _canceledOrderServices = canceledOrderServices;
        _regionRepository = regionRepository;
        _clientServices = clientServices;
    }

    [HttpGet]
    public async Task<IActionResult> CancelOrderByIdAsync(long id, CancellationToken token)
    {
        var order = await _orderRepository.FindAsync(id, token);

        if (order == null)
            return NotFound($"Order with id: {id} not found");

        if (ForbiddenToCancelStates.Contains(order.State))
            return BadRequest($"Cannot cancel order with id:{id} in state {order.State}");

        await _canceledOrderServices.CanceledOrderInLogisticsSimulator(id, token);

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
            {
                return NotFound($"Region with id: {regionId} not found");
            };
        }

        var orders = await _orderRepository.GetOrdersListWithFiltersByPageAsync(request, token);
        var response = new List<OrderResponse>();

        foreach (var order in orders)
        {
            var client = await _clientServices.GetClientAsync(order.ClientId, token);

            response.Add(new OrderResponse()
            {
                Id = order.Id,
                Status = order.State,
                DateCreate = order.DateCreate,
                CountProduct = order.CountProduct,
                Telephone = client.Telephone,
                TotalSumm = order.TotalSumm,
                TotalWeight = order.TotalWeight,
                DeliveryAddress = order.DeliveryAddress,
                Client = new ClientName()
                {
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                }
            });
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> GetOrdersListByRegionsAndDateTimeAsync([FromBody] OrdersListByRegionsAndDateTimeRequest request, CancellationToken token)
    {
        foreach (var regionId in request.regionIds)
        {
            if (!await _regionRepository.IsExistsAsync(regionId, token))
                return NotFound($"Region with id: {regionId} not found");

        }

        var orders = await _orderRepository.GetManyAsync(x => x.DateCreate >= request.startPeriod &&
                                                   request.regionIds.Count > 0 && request.regionIds.Contains(x.DeliveryAddress.RegionId),
                                                   token);

        var regionsGrouping = orders.GroupBy(x => x.DeliveryAddress.RegionId);
        var ordersinRegions = new List<OrdersInRegionResponse>();
        var clients = await _clientServices.GetClientsAsync(token);

        foreach (var region in regionsGrouping)
        {
            long totalSum = 0;
            long totalWeight = 0;

            region.ToList().ForEach(x => { totalSum += x.TotalSumm; });
            region.ToList().ForEach(x => { totalWeight += x.TotalWeight; });

            ordersinRegions.Add(new OrdersInRegionResponse()
            {
                RegionName = region.First().DeliveryAddress.Region.Name,
                CountOrders = region.Count(),
                TotalWeight = totalWeight,
                TotalSumOrders = totalSum,
                CountClients = clients.Where(x => x.DefaultAdress.Region == region.First().DeliveryAddress.Region.Name).Count()
            });
        }

        return Ok(ordersinRegions);
    }

    [HttpPost]
    public async Task<IActionResult> GetOrdersForClientByTimeAsync([FromBody] OrdersForClientByTimeRequest request, CancellationToken token)
    {
        var client = await _clientServices.GetClientAsync(request.ClientId, token);

        if (client == null)
            return NotFound($"Customer with id: {request.ClientId} not found");

        var orders = await _orderRepository.GetManyAsync(x => x.DateCreate >= request.StartPeriod && x.ClientId == request.ClientId, token);
        var skip = request.OnPage * (request.CurrentPage - 1);
        orders = orders.Skip(skip).Take(request.OnPage).ToArray();

        var response = new List<OrderResponse>();
        foreach (var order in orders)
        {
            response.Add(new OrderResponse()
            {
                Id = order.Id,
                Status = order.State,
                DateCreate = order.DateCreate,
                CountProduct = order.CountProduct,
                Telephone = client.Telephone,
                TotalSumm = order.TotalSumm,
                TotalWeight = order.TotalWeight,
                DeliveryAddress = order.DeliveryAddress,
                Client = new ClientName()
                {
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                }
            });
        }

        return Ok(response);
    }
}