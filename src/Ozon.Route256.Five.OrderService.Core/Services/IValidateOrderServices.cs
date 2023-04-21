using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Core.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Core.Services;

public interface IValidateOrderServices
{
    /// <summary>
    ///  Проверка можно ли отменить заказ
    /// </summary>
    public bool IsCanCancelOrder(OrderState state);

    /// <summary>
    ///  Проверка заказа на валидность
    /// </summary>
    public bool IsOrderValid(Order order);
}
