using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection.ShardConnection;

namespace Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataBaseConnection(
    this IServiceCollection services)
    {

        SqlMapper.AddTypeHandler(AddressObjectHandler.Instance);
        services.AddScoped<IShardingRule<long>, RoundRobinLongShardingRule>();
        services.AddScoped<IShardingRule<string>, RoundRobinStringShardingRule>();
        services.AddScoped<IShardConnectionFactory, ShardConnectionFactory>();

        return services;
    }
}
