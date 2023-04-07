using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json;
using Ozon.Route256.Five.OrderService.Infrastructure.Kafka.Producers;

namespace Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrder;

public class NewOrderKafkaProducer : INewOrderKafkaProducer
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IKafkaProducer _kafkaProducer;
    private readonly OrderEventSettings _orderEventSettings;

    public NewOrderKafkaProducer(IKafkaProducer kafkaProducer, IOptionsSnapshot<OrderEventSettings> optionsSnapshot)
    {
        _kafkaProducer = kafkaProducer;
        _orderEventSettings = optionsSnapshot.Value;
    }

    public Task PublishToKafka(NewOrderRequest request, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(_orderEventSettings.Topic))
            throw new Exception($"Topic for {nameof(NewOrderKafkaProducer)} is empty");

        var value = JsonSerializer.Serialize(request, JsonSerializerOptions);
        return _kafkaProducer.SendMessage(request.OrderId.ToString(), value, _orderEventSettings.Topic, token);
    }
}
