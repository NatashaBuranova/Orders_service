using Microsoft.Extensions.Configuration;
using Npgsql;
using Ozon.Route256.Five.OrderService.Infrastructure.ClientBalancing;
using Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection;
using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection.ShardConnection;

public class ShardConnectionFactory : IShardConnectionFactory
{
    private readonly IDbStore _dbStore;
    private readonly IShardingRule<long> _longShardingRule;
    private readonly IShardingRule<string> _stringShardingRule;
    private readonly int _bucketsCount;
    private readonly string _connectionString;

    public ShardConnectionFactory(IDbStore dbStore,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule,
        IConfiguration configuration)
    {
        _dbStore = dbStore;
        _longShardingRule = longShardingRule;
        _stringShardingRule = stringShardingRule;
        _bucketsCount = configuration.GetSection("DataBase:BacketCount").Get<int>();
        _connectionString = configuration.GetSection("DataBase:ConnectionString").Get<string>();
    }

    public async Task<string> GetConnectionStringAsync(int bucketId)
    {
        var result = await _dbStore.GetEndpointByBucketAsync(bucketId);
        return $"Server={result.HostAndPort};{_connectionString}";
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
