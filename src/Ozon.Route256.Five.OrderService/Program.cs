using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ozon.Route256.Five.OrderService;
using Ozon.Route256.Five.OrderService.Extentions;

await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHost(w =>
        w.ConfigureKestrel(options =>
            options.ConfigureEndpointDefaults(x => x.Protocols = HttpProtocols.Http2)))
    .ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); })
    .Build()
    .RunAndMigrateAsync();

