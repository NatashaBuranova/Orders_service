using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;
using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Repositories;

public interface IOrderRepository
{
    Task<Order?> FindAsync(long orderId, CancellationToken token);
    Task UpdateAsync(Order order, CancellationToken token);
    Task<bool> IsExistsAsync(long orderId, CancellationToken token);
    Task<Order[]> GetOrdersListWithFiltersByPageAsync(OrdersListWithFiltersRequest filters, CancellationToken token);
    Task<Order[]> GetManyAsync(Func<Order, bool> where, CancellationToken token);
}
