using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Helpers;

public class CanceledOrderServices : ICancelOrderServices
{
    private readonly LogisticsSimulatorService.LogisticsSimulatorService.LogisticsSimulatorServiceClient _client;

    public CanceledOrderServices(LogisticsSimulatorService.LogisticsSimulatorService.LogisticsSimulatorServiceClient client)
    {
        _client = client;
    }

    public async Task CancelOrderInLogisticsSimulator(long orderId, CancellationToken token)
    {
        var result = await _client.OrderCancelAsync(new LogisticsSimulatorService.Order
        {
            Id = orderId
        },
        cancellationToken: token);

        if (!result.Success)
            throw new Exception(result.Error);
    }
}
