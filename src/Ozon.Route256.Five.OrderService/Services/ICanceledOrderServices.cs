namespace Ozon.Route256.Five.OrderService.Services;

public interface ICanceledOrderServices
{
    /// <summary>
    /// Отмена заказа в симуляторе логистики
    /// </summary>
    Task CanceledOrderInLogisticsSimulator(long orderId, CancellationToken token);
}
