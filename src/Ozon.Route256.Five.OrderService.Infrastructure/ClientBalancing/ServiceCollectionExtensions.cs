using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;

namespace Ozon.Route256.Five.OrderService.Infrastructure.ClientBalancing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceDiscovery(
        this IServiceCollection services,
        Action<GrpcClientFactoryOptions> configureOptions)
    {
        services.AddSingleton<IDbStore, DbStore>();
        services.AddHostedService<SdConsumerHostedService>();
        services.AddGrpcClient<SdService.SdServiceClient>(configureOptions);

        return services;
    }
}
