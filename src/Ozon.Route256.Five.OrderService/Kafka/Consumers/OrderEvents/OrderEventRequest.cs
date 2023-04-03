namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;

public record OrderEventRequest(long Id, DateTimeOffset UpdateDate, string NewState);

