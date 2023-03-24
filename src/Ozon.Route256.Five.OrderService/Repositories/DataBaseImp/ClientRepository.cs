using Dapper;
using Ozon.Route256.Five.OrderService.Infrastructure;

namespace Ozon.Route256.Five.OrderService.Repositories.DataBaseImp;

public class ClientRepository : IClientRepository
{
    private readonly string _IsExistQuery = @"SELECT EXISTS (SELECT * FROM customer where id = @clientId)";

    private readonly IPostgresConnectionFactory _connectionFactory;

    public ClientRepository(IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> IsExistsAsync(long clientId, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<bool>(_IsExistQuery, new { clientId });
        return result;
    }
}
