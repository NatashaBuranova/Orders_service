using Microsoft.Extensions.Configuration;
using Npgsql;
using Ozon.Route256.Five.OrderService.Infrastructure.ClientBalancing;
using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection.Connection;

public class PostgresConnectionFactory : IPostgresConnectionFactory
{
    private readonly IDbStore _dbStore;
    private readonly string _configurationString;

    public PostgresConnectionFactory(IDbStore dbStore, IConfiguration configuration)
    {
        _dbStore = dbStore;
        _configurationString = configuration.GetSection("DataBase:ConnectionString").Get<string>();
    }


    public async Task<string> GetConnectionStringAsync()
    {
        return _configurationString;
    }

    public async Task<DbConnection> GetConnectionAsync()
    {
        var connectionString = await GetConnectionStringAsync();
        return new NpgsqlConnection(connectionString);
    }
}
