namespace Ozon.Route256.Five.OrderService.Services;

public interface ICancelOrderServices
{
    /// <summary>
    /// Отмена заказа в симуляторе логистики
    /// </summary>
    Task CancelOrderInLogisticsSimulator(long orderId, CancellationToken token);
}
