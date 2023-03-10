namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

public record OrdersListInRegionsByTimeRequest(DateTimeOffset startPeriod, List<long> regionIds);
