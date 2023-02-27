
using Ozon.Route256.Five.OrderService;

await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); })
    .Build()
    .RunAsync();

