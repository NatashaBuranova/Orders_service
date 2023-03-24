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
            DeliveryAddress = new Adress()
            {
                RegionId = Faker.Random.Int(1, 3),
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

    public Task<bool> IsExistsAsync(long orderId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<bool>(token);

        return Task.FromResult(_orders.ContainsKey(orderId));
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

        var orders = _orders.Values.Where(x => ((filters.RegionFilterIds.Count > 0 && filters.RegionFilterIds.Contains(x.DeliveryAddress.RegionId)) ||
                                                filters.RegionFilterIds.Count == 0) &&
                                                ((filters.TypeOrder.HasValue && x.Type == filters.TypeOrder.Value) || !filters.TypeOrder.HasValue));

        if (filters.IsOrderByFilter)
            orders = orders.OrderBy(x => x.DeliveryAddress.RegionId);

        var skip = filters.OnPage * (filters.CurrentPage - 1);

        return Task.FromResult(orders.Skip(skip).Take(filters.OnPage).ToArray());
    }

    public Task<Order[]> GetManyAsync(Func<Order, bool> where, CancellationToken token)
    {

        if (token.IsCancellationRequested)
            return Task.FromCanceled<Order[]>(token);

        return Task.FromResult(_orders.Values.Where(where).ToArray());
    }
}
