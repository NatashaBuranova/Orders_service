using Ozon.Route256.Five.OrderService.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

public record OrdersListWithFiltersRequest(List<long> RegionFilterIds,
    bool IsOrderByFilter,
    OrderType? TypeOrder,
    int PageSize,
    int CurrentPage = 1);
