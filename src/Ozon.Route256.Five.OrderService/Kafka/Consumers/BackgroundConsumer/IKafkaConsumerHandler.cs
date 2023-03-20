namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;

public interface IKafkaConsumerHandler<in TKey, in TValue>
{
    public Task Handle(TKey key, TValue message, CancellationToken token);
}
