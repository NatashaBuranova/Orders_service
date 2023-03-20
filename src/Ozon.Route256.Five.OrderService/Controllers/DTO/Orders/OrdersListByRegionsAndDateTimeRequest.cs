namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

public record OrdersListByRegionsAndDateTimeRequest(DateTimeOffset startPeriod, List<long> regionIds);
