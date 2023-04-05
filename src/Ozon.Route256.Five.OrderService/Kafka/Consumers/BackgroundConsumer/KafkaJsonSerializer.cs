using Confluent.Kafka;
using System.Text.Json;

namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;

public class KafkaJsonSerializer<TValue> : IDeserializer<TValue>
{
    public TValue Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) =>
        JsonSerializer.Deserialize<TValue>(data)!;
}

