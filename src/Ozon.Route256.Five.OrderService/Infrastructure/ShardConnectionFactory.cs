using Npgsql;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

public class ShardConnectionFactory : IShardConnectionFactory
{
    private readonly IDbStore _dbStore;
    private readonly IShardingRule<long> _longShardingRule;
    private readonly IShardingRule<string> _stringShardingRule;
    private readonly int _bucketsCount;

    public ShardConnectionFactory(IDbStore dbStore,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule)
    {
        _dbStore = dbStore;
        _longShardingRule = longShardingRule;
        _stringShardingRule = stringShardingRule;
        _bucketsCount = _dbStore.BucketsCount;
    }

    public async Task<string> GetConnectionStringAsync(int bucketId)
    {
        var result = await _dbStore.GetEndpointByBucketAsync(bucketId);
        return $"Server={result.HostAndPort};Database=orders_db;User Id=admin;Password=admin;";
    }

    public async Task<DbConnection> GetConnectionByKeyAsync(
        long shardKey,
        CancellationToken token)
    {
        var bucketId = _longShardingRule.GetBucketId(shardKey, _bucketsCount);
        return await GetConnectionByBucketAsync(bucketId, token);
    }

    public async Task<DbConnection> GetConnectionByKeyAsync(
        string shardKey,
        CancellationToken token)
    {
        var bucketId = _stringShardingRule.GetBucketId(shardKey, _bucketsCount);
        return await GetConnectionByBucketAsync(bucketId, token);
    }

    public async Task<DbConnection> GetConnectionByBucketAsync(
        int bucketId,
        CancellationToken token)
    {
        var connectionString = await GetConnectionStringAsync(bucketId);
        return new ShardNpgsqlConnection(new NpgsqlConnection(connectionString), bucketId);
    }
}
