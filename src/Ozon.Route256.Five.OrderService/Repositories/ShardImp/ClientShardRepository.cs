using Dapper;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.Infrastructure;

namespace Ozon.Route256.Five.OrderService.Repositories.ShardImp;

public class ClientShardRepository : IClientRepository
{

    private readonly string _insertQuery = $@"
INSERT INTO __bucket__.customer
    ( id,
    first_name, 
    last_name, 
    phone_number)
VALUES ( @{nameof(Models.Client.Id)},
@{nameof(Models.Client.FirstName)},
@{nameof(Models.Client.LastName)},
@{nameof(Models.Client.Telephone)})";

    private readonly IShardConnectionFactory _connectionFactory;
    private readonly IShardingRule<long> _shardingRule;
    private readonly int _bucketsCount;

    public ClientShardRepository(IShardConnectionFactory connectionFactory,
        IShardingRule<long> shardingRule,
        IDbStore dbStore)
    {
        _connectionFactory = connectionFactory;
        _shardingRule = shardingRule;
        _bucketsCount = dbStore.BucketsCount;
    }

    public async Task InsertAsync(Models.Client newClient, long regionId, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionByKeyAsync(regionId, token);
        await connection.ExecuteAsync(_insertQuery, newClient);
    }

    public Task<bool> IsExistsAsync(long clientId, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
