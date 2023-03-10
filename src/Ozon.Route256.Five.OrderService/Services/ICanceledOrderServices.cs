namespace Ozon.Route256.Five.OrderService.Services;

public interface ICanceledOrderServices
{
    Task CanceledOrderInLogisticsSimulator(long orderId, CancellationToken token);
}
