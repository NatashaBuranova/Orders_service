using Npgsql;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

public class PostgresConnectionFactory : IPostgresConnectionFactory
{
    private readonly IDbStore _dbStore;

    public PostgresConnectionFactory(IDbStore dbStore)
        => _dbStore = dbStore;

    public async Task<string> GetConnectionStringAsync()
    {
        return $"Server=localhost;Port=5433;Database=orders_db;User Id=admin;Password=admin;";
    }

    public async Task<DbConnection> GetConnectionAsync()
    {
        var connectionString = await GetConnectionStringAsync();
        return new NpgsqlConnection(connectionString);
    }
}
