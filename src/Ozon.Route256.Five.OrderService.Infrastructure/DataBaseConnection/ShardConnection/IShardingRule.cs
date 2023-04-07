namespace Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection.ShardConnection;

public interface IShardingRule<TShardKey>
{
    int GetBucketId(
        TShardKey key,
        int bucketsCount);
}