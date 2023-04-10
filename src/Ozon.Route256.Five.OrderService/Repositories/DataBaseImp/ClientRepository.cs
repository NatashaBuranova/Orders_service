using Dapper;
using Ozon.Route256.Five.OrderService.Infrastructure;

namespace Ozon.Route256.Five.OrderService.Repositories.DataBaseImp;

public class ClientRepository : IClientRepository
{
    private readonly string _IsExistQuery = @"SELECT EXISTS (SELECT * FROM customer where id = @clientId)";

    private readonly string _insertQuery = $@"
INSERT INTO customer
    ( id,
    first_name, 
    last_name, 
    phone_number)
VALUES ( @{nameof(Models.Client.Id)},
@{nameof(Models.Client.FirstName)},
@{nameof(Models.Client.LastName)},
@{nameof(Models.Client.Telephone)})";

    private readonly IPostgresConnectionFactory _connectionFactory;

    public ClientRepository(IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InsertAsync(Models.Client newClient, long regionId, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        await connection.ExecuteAsync(_insertQuery, newClient);
    }

    public async Task<bool> IsExistsAsync(long clientId, CancellationToken token)
    {
        await using var connection = await _connectionFactory.GetConnectionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<bool>(_IsExistQuery, new { clientId });
        return result;
    }
}
