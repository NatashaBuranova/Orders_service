namespace Ozon.Route256.Five.OrderService.Web.Api.DTO.Orders;

public record OrdersListByRegionsAndDateTimeRequest(DateTimeOffset startPeriod, List<long> regionIds);
