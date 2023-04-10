namespace Ozon.Route256.Five.OrderService.Infrastructure;

public interface IShardingRule<TShardKey>
{
    int GetBucketId(
        TShardKey key,
        int bucketsCount);
}