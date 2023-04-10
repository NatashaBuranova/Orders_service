using Ozon.Route256.Five.OrderService.Infrastructure;

namespace Ozon.Route256.Five.OrderService.Extentions;

public static class HostExtention
{
    public static async Task RunAndMigrateAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var sdClient = scope.ServiceProvider.GetRequiredService<SdService.SdServiceClient>();
        var migratorRunner = new ShardMigratorRunner(sdClient);
        await migratorRunner.Migrate();
    }
}
