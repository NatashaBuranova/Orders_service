namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

public record OrdersForClientByTimeRequest(long ClientId, DateTime StartPeriod, int OnPage, int CurrentPage = 1);

