using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Core.Services;
using Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrder;

namespace Ozon.Route256.Five.OrderService.Services;

public class SendNewOrder : ISendNewOrder
{
    private readonly INewOrderKafkaProducer _newOrderKafkaPublisher;
    private readonly IValidateOrderServices _validateOrderServices;

    public SendNewOrder(INewOrderKafkaProducer newOrderKafkaPublisher, IValidateOrderServices validateOrderServices)
    {
        _newOrderKafkaPublisher = newOrderKafkaPublisher;
        _validateOrderServices = validateOrderServices;
    }


    public async Task SendValidOrder(Order order, CancellationToken token)
    {
        if (_validateOrderServices.IsOrderValid(order))
            await _newOrderKafkaPublisher.PublishToKafka(new NewOrderRequest(order.Id), token);
    }
}
