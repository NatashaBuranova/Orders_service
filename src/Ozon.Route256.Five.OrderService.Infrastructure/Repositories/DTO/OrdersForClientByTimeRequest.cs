namespace Ozon.Route256.Five.OrderService.Infrastructure.Repositories.DTO;

public record OrdersForClientByTimeRepositoryRequest(int ClientId, DateTimeOffset StartPeriod, int PageSize, int CurrentPage = 1);
