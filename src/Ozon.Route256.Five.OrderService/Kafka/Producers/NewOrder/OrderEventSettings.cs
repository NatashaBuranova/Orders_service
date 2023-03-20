namespace Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrder;

public class OrderEventSettings
{
    public const string Sections = "Kafka:NewOrderProducer";
    public string? Topic { get; set; }
}
