using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Infrastructure.Repositories.DTO;
using Ozon.Route256.Five.OrderService.Web.Api.DTO.Orders;

namespace Ozon.Route256.Five.OrderService.Web.Api.Helpers;

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

    public static OrdersForClientByTimeRepositoryRequest ToOrdersForClientByTimeRepositoryRequest(OrdersForClientByTimeRequest request)
    {
        return new OrdersForClientByTimeRepositoryRequest(
            ClientId: request.ClientId,
            StartPeriod: request.StartPeriod,
            PageSize: request.PageSize,
            CurrentPage: request.CurrentPage);
    }

    public static OrdersListWithFiltersRepositoryRequest ToOrdersListWithFiltersRepositoryRequest(OrdersListWithFiltersRequest request)
    {
        return new OrdersListWithFiltersRepositoryRequest(
            RegionFilterIds: request.RegionFilterIds,
            IsOrderByFilter: request.IsOrderByFilter,
            TypeOrder: request.TypeOrder,
            PageSize: request.PageSize,
            CurrentPage: request.CurrentPage);
    }
}
