using Dapper;
using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection.ShardConnection;

namespace Ozon.Route256.Five.OrderService.Repositories.ShardImp;

public class ClientShardRepository : IClientRepository
{

    private readonly string _insertQuery = $@"
INSERT INTO __bucket__.customer
    ( id,
    first_name, 
    last_name, 
    phone_number)
VALUES ( @{nameof(Client.Id)},
@{nameof(Client.FirstName)},
@{nameof(Client.LastName)},
@{nameof(Client.Telephone)})";

    private readonly string _insertIndexQuery = @"
INSERT into  __bucket__.index_region_client (
    region_id,
    client_id)
VALUES (
    @RegionId,
    @ClientId)";

    private readonly string _indexQuery = @"
select region_id from __bucket__.index_region_client
where client_id = @ClientId";

    private readonly IShardConnectionFactory _connectionFactory;

    public ClientShardRepository(IShardConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InsertAsync(Client newClient, long regionId, CancellationToken token)
    {
        await using var connectionIndex = await _connectionFactory.GetConnectionByKeyAsync(newClient.Id, token);
        await connectionIndex.ExecuteAsync(_insertIndexQuery, new { ClientId = newClient.Id, RegionId = regionId });

        await using var connection = await _connectionFactory.GetConnectionByKeyAsync(regionId, token);
        await connection.ExecuteAsync(_insertQuery, newClient);
    }

    public async Task<bool> IsExistsAsync(long clientId, CancellationToken token)
    {
        await using var connectionIndex = await _connectionFactory.GetConnectionByKeyAsync(clientId, token);
        var ids = await connectionIndex.QueryAsync<long>(_indexQuery, new { clientId });

        if (!ids.Any()) return false;

        return true;
    }
}
