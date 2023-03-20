using Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;
using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Repositories;

public interface IOrderRepository
{
    /// <summary>
    ///  Нахождение заказа по ИД
    /// </summary>
    Task<Order?> FindAsync(long orderId, CancellationToken token);

    /// <summary>
    ///  Создание заказа
    /// </summary>
    public Task InsertAsync(Order newOrder, CancellationToken token);

    /// <summary>
    /// Обновление заказа
    /// </summary>
    Task UpdateAsync(Order order, CancellationToken token);

    /// <summary>
    ///  Проверка на существование заказа
    /// </summary>
    Task<bool> IsExistsAsync(long orderId, CancellationToken token);

    /// <summary>
    ///  Постраничное получение заказов с фильтрацией по регионам и типу заказа
    /// </summary>
    Task<Order[]> GetOrdersListWithFiltersByPageAsync(OrdersListWithFiltersRequest filters, CancellationToken token);

    /// <summary>
    ///  Получение заказов, соответсвующих условию
    /// </summary>
    Task<Order[]> GetManyAsync(Func<Order, bool> where, CancellationToken token);
}
