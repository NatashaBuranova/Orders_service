using Ozon.Route256.Five.OrderService.Core.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Web.Api.DTO.Orders;

public record OrdersListWithFiltersRequest(List<long> RegionFilterIds,
    bool IsOrderByFilter,
    OrderType? TypeOrder,
    int PageSize,
    int CurrentPage = 1);
