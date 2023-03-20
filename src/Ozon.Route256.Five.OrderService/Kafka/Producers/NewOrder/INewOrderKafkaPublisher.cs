namespace Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrder;

public interface INewOrderKafkaPublisher
{
    Task PublishToKafka(NewOrderDTO newOrder, CancellationToken token);
}
