namespace Ozon.Route256.Five.OrderService.DateTimeProvider;

public class LocalDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset CurrentDateTimeOffsetUtc => DateTimeOffset.UtcNow;
}
