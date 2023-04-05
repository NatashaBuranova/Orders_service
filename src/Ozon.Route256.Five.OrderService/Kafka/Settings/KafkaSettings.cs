namespace Ozon.Route256.Five.OrderService.Kafka.Settings;

public class KafkaSettings
{
    public string? BootstrapServers { get; set; }
    public string? GroupId { get; set; }
    public int TimeoutForRetryInSecond { get; set; } = 2;
}

