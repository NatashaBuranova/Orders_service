namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

public record OrdersListWithFiltersRequest(List<long> RegionFilterIds, bool IsOrderByFilter, int TypeOrder, int OnPage, int CurrentPage = 1);
