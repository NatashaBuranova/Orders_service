using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ozon.Route256.Five.OrderService;
using Ozon.Route256.Five.OrderService.Extentions;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHost(w =>
        w.ConfigureKestrel(options =>
            options.ConfigureEndpointDefaults(x => x.Protocols = HttpProtocols.Http2)))
    .ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); })
    .Build();

var isNeedMigrate = bool.TryParse(Environment.GetEnvironmentVariable("ISNEEDMIGRATE"), out var migrate) && migrate;

if (isNeedMigrate)
{
    await host.RunAndMigrateAsync();
}
else
{
    await host.RunAsync();
}

