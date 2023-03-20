using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrder;

public class NewOrderKafkaPublisher : INewOrderKafkaPublisher
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IKafkaProducer _kafkaProducer;
    private readonly OrderEventSettings _orderEventSettings;

    public NewOrderKafkaPublisher(IKafkaProducer kafkaProducer, IOptionsSnapshot<OrderEventSettings> optionsSnapshot)
    {
        _kafkaProducer = kafkaProducer;
        _orderEventSettings = optionsSnapshot.Value;
    }

    public Task PublishToKafka(NewOrderDTO dto, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(_orderEventSettings.Topic))
            throw new Exception($"Topic for {nameof(NewOrderKafkaPublisher)} is empty");

        var value = JsonSerializer.Serialize(dto, JsonSerializerOptions);
        return _kafkaProducer.SendMessage(dto.OrderId.ToString(), value, _orderEventSettings.Topic, token);
    }
}
