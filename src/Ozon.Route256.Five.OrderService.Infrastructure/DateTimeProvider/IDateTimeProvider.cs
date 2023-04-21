namespace Ozon.Route256.Five.OrderService.DateTimeProvider;

public interface IDateTimeProvider
{
    DateTimeOffset CurrentDateTimeOffsetUtc { get; }
}

