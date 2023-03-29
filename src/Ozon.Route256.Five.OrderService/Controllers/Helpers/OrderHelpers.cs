using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;
using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Controllers.Helpers;

public static class OrderHelpers
{
    public static List<OrderResponse> GetOrderResponse(Order[] orders)
    {
        return orders.Select(order => new OrderResponse()
        {
            Id = order.Id,
            Status = order.State,
            DateCreate = order.DateCreate,
            CountProduct = order.CountProduct,
            Telephone = order.Client?.Telephone,
            TotalSumm = order.TotalSumm,
            TotalWeight = order.TotalWeight,
            DeliveryAddress = order.DeliveryAddress,
            Client = new ClientName()
            {
                FirstName = order.Client?.FirstName,
                LastName = order.Client?.LastName,
            }
        }).ToList();
    }

    public static List<OrdersInRegionResponse> GroupOrderByRegions(Order[] orders)
    {
        return orders.GroupBy(x => x.RegionId)
            .Select(x => new OrdersInRegionResponse
            {
                RegionName = x.First()?.DeliveryAddress?.Region,
                CountOrders = x.Count(),
                CountClients = x.Select(x => x.ClientId).Distinct().Count(),
                TotalSumOrders = x.Sum(t => t.TotalSumm),
                TotalWeight = x.Sum(t => t.TotalWeight)
            })
            .ToList();
    }
}
