using Ozon.Route256.Five.OrderService.Core.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Infrastructure.Repositories.DTO;

public record OrdersListWithFiltersRepositoryRequest(List<long> RegionFilterIds,
bool IsOrderByFilter,
OrderType? TypeOrder,
int PageSize,
int CurrentPage = 1);
