using Ozon.Route256.Five.OrderService.Core.Helpers;
using Ozon.Route256.Five.OrderService.Infrastructure.Kafka.Consumers.BackgroundConsumer;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;
using Ozon.Route256.Five.OrderService.Repositories;

namespace Ozon.Route256.Five.OrderService.Consumers.Kafka.OrderEvents;

public class OrderEventsConsumerHandler : IKafkaConsumerHandler<string, OrderEventRequest>
{
    private readonly IOrderRepository _orderRepository;

    public OrderEventsConsumerHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task HandleAsync(string key, OrderEventRequest message, CancellationToken token)
    {
        var order = await _orderRepository.FindAsync(message.Id, token);

        if (order == null)
            throw new Exception($"Order with Id:{message.Id} not found");

        order.State = StateOrderMapping.GetStateOrderFromName(message.NewState);
        order.DateUpdate = message.UpdateDate;

        await _orderRepository.UpdateAsync(order, token);
    }
}

