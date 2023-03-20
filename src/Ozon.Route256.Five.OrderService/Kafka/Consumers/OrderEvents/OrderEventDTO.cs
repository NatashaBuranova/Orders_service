namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;

public record OrderEventDTO(long Id, DateTimeOffset UpdateDate, string NewState);

