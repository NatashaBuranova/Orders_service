using System.Data.Common;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

public interface IPostgresConnectionFactory
{
    Task<string> GetConnectionStringAsync();
    Task<DbConnection> GetConnectionAsync();
}
