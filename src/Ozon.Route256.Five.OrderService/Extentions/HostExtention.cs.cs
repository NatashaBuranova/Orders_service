using FluentMigrator.Runner;

namespace Ozon.Route256.Five.OrderService.Extentions;

public static class HostExtention
{
    public static async Task RunAndMigrateAsync(this IHost host)
    {
        await host
            .MigrateAsync()
            .RunAsync();
    }

    private static IHost MigrateAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        return host;
    }
}
