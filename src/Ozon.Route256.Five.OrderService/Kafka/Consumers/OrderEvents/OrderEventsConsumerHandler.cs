using Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;
using Ozon.Route256.Five.OrderService.Models.Enums;
using Ozon.Route256.Five.OrderService.Repositories;

namespace Ozon.Route256.Five.OrderService.Consumers.Kafka.OrderEvents;

public class OrderEventsConsumerHandler : IKafkaConsumerHandler<string, OrderEventDTO>
{
    private readonly IOrderRepository _orderRepository;

    public OrderEventsConsumerHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(string key, OrderEventDTO message, CancellationToken token)
    {
        var order = await _orderRepository.FindAsync(message.Id, token);

        if (order == null) throw new Exception($"Order with Id:{message.Id} not found");

        order.State = GetStateOrder(message.NewState);
        order.DateUpdate = message.UpdateDate;

        await _orderRepository.UpdateAsync(order, token);
    }

    private static OrderState GetStateOrder(string stateName)
    {
        OrderState state = OrderState.Cancelled;

        switch (stateName)
        {
            case "Created":
                state = OrderState.Created;
                break;
            case "SentToCustomer":
                state = OrderState.SentToCustomer;
                break;
            case "Delivered":
                state = OrderState.Delivered;
                break;
            case "Lost":
                state = OrderState.Lost;
                break;
            case "Cancelled":
                state = OrderState.Cancelled;
                break;
        }

        return state;
    }
}

