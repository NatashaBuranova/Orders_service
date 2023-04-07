using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Repositories.ShardImp;

namespace Ozon.Route256.Five.OrderService.Infrastructure.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderShardRepository>();
        services.AddScoped<IRegionRepository, RegionShardRepository>();
        services.AddScoped<IClientRepository, ClientShardRepository>();

        return services;
    }
}
