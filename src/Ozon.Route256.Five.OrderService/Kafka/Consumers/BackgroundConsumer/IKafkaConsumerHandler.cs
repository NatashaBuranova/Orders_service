namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;

public interface IKafkaConsumerHandler<in TKey, in TValue>
{
    public Task HandleAsync(TKey key, TValue message, CancellationToken token);
}
