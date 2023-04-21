using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Infrastructure.Repositories.DTO;
using System.Collections.Concurrent;

namespace Ozon.Route256.Five.OrderService.Repositories.ImMemoryImp;

public class OrderInMemoryRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<long, Order> _orders = new(2, 10);

    public OrderInMemoryRepository() { }

    public Task<Order?> FindAsync(long orderId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<Order?>(token);

        var result = _orders.TryGetValue(orderId, out var order) ? order : null;

        return Task.FromResult(result);
    }

    public Task InsertAsync(Order newOrder, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled(token);

        if (_orders.ContainsKey(newOrder.Id))
            throw new Exception($"Order with id {newOrder.Id} already exists");

        _orders[newOrder.Id] = newOrder;

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled(token);

        _orders[order.Id] = order;

        return Task.CompletedTask;
    }

    public Task<Order[]> GetOrdersListWithFiltersByPageAsync(OrdersListWithFiltersRepositoryRequest filters, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<Order[]>(token);

        var orders = _orders.Values.Where(x => filters.RegionFilterIds.Count == 0 || filters.RegionFilterIds.Contains(x.RegionId))
                                   .Where(x => !filters.TypeOrder.HasValue || x.Type == filters.TypeOrder.Value);

        if (filters.IsOrderByFilter)
            orders = orders.OrderBy(x => x.RegionId);

        var skip = filters.PageSize * (filters.CurrentPage - 1);

        return Task.FromResult(orders.Skip(skip).Take(filters.PageSize).ToArray());
    }

    public Task<Order[]> GetOrdersForClientByTimePerPageAsync(OrdersForClientByTimeRepositoryRequest filters, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<Order[]>(token);

        var orders = _orders.Values.Where(x => x.DateCreate >= filters.StartPeriod)
                                   .Where(x => x.ClientId == filters.ClientId);

        var skip = filters.PageSize * (filters.CurrentPage - 1);

        return Task.FromResult(orders.Skip(skip).Take(filters.PageSize).ToArray());
    }

    public Task<Order[]> GetOrdersListByRegionsAndDateTimeAsync(DateTimeOffset dateStart, List<long> regionIds, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<Order[]>(token);

        var orders = _orders.Values.Where(x => x.DateCreate >= dateStart)
                                   .Where(x => regionIds.Contains(x.RegionId) || regionIds.Count == 0)
                                   .ToArray();

        return Task.FromResult(orders);
    }

}
