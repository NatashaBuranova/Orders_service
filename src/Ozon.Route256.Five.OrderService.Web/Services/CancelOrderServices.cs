using Ozon.Route256.Five.OrderService.LogisticsSimulatorGrpsService;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Helpers;

public class CancelOrderServices : ICancelOrderServices
{
    private readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _client;

    public CancelOrderServices(LogisticsSimulatorService.LogisticsSimulatorServiceClient client)
    {
        _client = client;
    }

    public async Task CancelOrderInLogisticsSimulator(long orderId, CancellationToken token)
    {
        var result = await _client.OrderCancelAsync(new Order
        {
            Id = orderId
        },
        cancellationToken: token);

        if (!result.Success)
            throw new Exception(result.Error);
    }
}
