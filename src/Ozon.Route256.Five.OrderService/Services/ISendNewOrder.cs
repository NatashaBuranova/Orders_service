using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Services;

public interface ISendNewOrder
{
    /// <summary>
    /// Отправка валидных заказов в кафку
    /// </summary>
    Task SendValidOrder(Order order, CancellationToken token);
}
