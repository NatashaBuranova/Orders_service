namespace Ozon.Route256.Five.OrderService.Infrastructure.Kafka.Consumers.BackgroundConsumer;

public interface IKafkaConsumerHandler<in TKey, in TValue>
{
    public Task HandleAsync(TKey key, TValue message, CancellationToken token);
}
