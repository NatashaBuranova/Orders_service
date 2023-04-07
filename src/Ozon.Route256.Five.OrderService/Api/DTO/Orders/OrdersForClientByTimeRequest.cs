namespace Ozon.Route256.Five.OrderService.Web.Api.DTO.Orders;

public record OrdersForClientByTimeRequest(int ClientId, DateTimeOffset StartPeriod, int PageSize, int CurrentPage = 1);

