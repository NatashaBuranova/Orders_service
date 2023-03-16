namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

public record OrdersListByRegionsAndDateTimeRequest(DateTime startPeriod, List<long> regionIds);

