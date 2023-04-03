namespace Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrder;

public interface INewOrderKafkaProducer
{
    Task PublishToKafka(NewOrderRequest newOrder, CancellationToken token);
}
