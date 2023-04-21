using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.OrderService.Infrastructure.ClientBalancing;
using Ozon.Route256.Five.OrderService.Infrastructure.Migrations;

namespace Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection.ShardConnection;

public class ShardMigratorRunner
{
    private readonly SdService.SdServiceClient _client;

    public ShardMigratorRunner(
        SdService.SdServiceClient client)
    {
        _client = client;
    }

    public async Task Migrate()
    {
        var endpoints = await GetEndpoints();

        foreach (var info in endpoints)
        {
            foreach (var bucketId in info.Buckets)
            {
                var connectionString = CreateConnectionString(info);
                var schema = $"bucket_{bucketId}";
                var serviceProvider = CreateServices(connectionString, schema);

                using (var scope = serviceProvider.CreateScope())
                {
                    UpdateDatabase(serviceProvider);
                }
            }
        }
    }

    private void UpdateDatabase(IServiceProvider provider)
    {
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    private IServiceProvider CreateServices(string connectionString, string schema)
    {
        var provider = new ServiceCollection()
            .AddSingleton<IConventionSet>(new DefaultConventionSet(schema, null))
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithRunnerConventions(new MigrationRunnerConventions())
                .WithMigrationsIn(typeof(CreateTableMigration).Assembly)
            )
            .BuildServiceProvider(false);
        return provider;
    }

    private string CreateConnectionString(
        DbEndpoint info) => $"Server={info.HostAndPort};Database=orders_db;User Id=admin;Password=admin;";

    private async Task<DbEndpoint[]> GetEndpoints()
    {
        var token = CancellationToken.None;
        using var stream = _client.DbResources(new DbResourcesRequest { ClusterName = "orders-cluster" }, cancellationToken: token);

        await stream.ResponseStream.MoveNext(CancellationToken.None);
        var response = stream.ResponseStream.Current;
        var endpoints = new List<DbEndpoint>(response.Replicas.Capacity);

        foreach (var replica in response.Replicas)
        {
            var endpoint = new DbEndpoint(
                $"{replica.Host}:{replica.Port}",
                DbReplicaType.Master,
                replica.Buckets.ToArray());
            endpoints.Add(endpoint);
        }

        return endpoints.ToArray();
    }

}