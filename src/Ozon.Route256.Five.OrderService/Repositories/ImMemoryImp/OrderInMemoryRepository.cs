using Bogus;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;
using Ozon.Route256.Five.OrderService.Models;
using System.Collections.Concurrent;

namespace Ozon.Route256.Five.OrderService.Repositories.ImMemoryImp;

public class OrderInMemoryRepository : IOrderRepository
{
    private static readonly Faker Faker = new();
    private readonly ConcurrentDictionary<long, Order> _orders = new(2, 10);

    public OrderInMemoryRepository()
    {
        var orders = Enumerable.Range(0, 10).Select(x => new Order()
        {
            Id = x,
            ClientId = x,
            CountProduct = Faker.Random.Int(1, 100),
            DateCreate = Faker.Date.Past(1),
            State = Models.Enums.OrderState.Created,
            Type = Models.Enums.OrderType.Mobile,
            DeliveryAddress = new Models.Address()
            {
                Region = Faker.Address.Country(),
                City = Faker.Address.City(),
                Street = Faker.Address.StreetName(),
                Building = Faker.Address.BuildingNumber(),
                Apartment = Faker.Random.Int(9, 999).ToString(),
                Latitude = Faker.Address.Latitude(),
                Longitude = Faker.Address.Longitude()
            }
        });

        foreach (var order in orders)
        {
            _orders[order.Id] = order;
        }
    }

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

    public Task<Order[]> GetOrdersListWithFiltersByPageAsync(OrdersListWithFiltersRequest filters, CancellationToken token)
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

    public Task<Order[]> GetOrdersForClientByTimePerPageAsync(OrdersForClientByTimeRequest filters, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<Order[]>(token);

        var orders = _orders.Values.Where(x => x.DateCreate >= filters.StartPeriod && x.ClientId == filters.ClientId).ToArray();

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
