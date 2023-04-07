using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection.Connection;

public interface IPostgresConnectionFactory
{
    Task<string> GetConnectionStringAsync();
    Task<DbConnection> GetConnectionAsync();
}
