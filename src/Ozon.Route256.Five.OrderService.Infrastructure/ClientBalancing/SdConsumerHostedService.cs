﻿using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ozon.Route256.Five.OrderService.Infrastructure.ClientBalancing;

public class SdConsumerHostedService : BackgroundService
{
    private readonly IDbStore _dbStore;
    private readonly SdService.SdServiceClient _client;
    private readonly ILogger<SdConsumerHostedService> _logger;

    public SdConsumerHostedService(IDbStore dbStore, SdService.SdServiceClient client, ILogger<SdConsumerHostedService> logger)
    {
        _dbStore = dbStore;
        _client = client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var stream = _client.DbResources(
                new DbResourcesRequest
                {
                    ClusterName = "orders-cluster"
                },
                cancellationToken: stoppingToken);

            try
            {
                while (await stream.ResponseStream.MoveNext(stoppingToken))
                {
                    _logger.LogDebug("Получены новые даныне из SD. Timestamp {Timestamp}", stream.ResponseStream.Current.LastUpdated.ToDateTime());
                    var response = stream.ResponseStream.Current;

                    var endpoints = new List<DbEndpoint>(response.Replicas.Capacity);

                    foreach (var replica in response.Replicas)
                    {
                        var endpoint = new DbEndpoint(
                            $"{replica.Host}:{replica.Port}",
                            GetDbReplicaType(replica.Type),
                            replica.Buckets.ToArray());
                        endpoints.Add(endpoint);
                    }

                    await _dbStore.UpdateEndpointsAsync(endpoints);
                }
            }
            catch (RpcException exc)
            {
                _logger.LogError(exc, "Не удалось связаться с SD");
            }
        }
    }

    private static DbReplicaType GetDbReplicaType(Replica.Types.ReplicaType replicaType)
    {
        return replicaType switch
        {
            Replica.Types.ReplicaType.Master => DbReplicaType.Master,
            Replica.Types.ReplicaType.Sync => DbReplicaType.Sync,
            Replica.Types.ReplicaType.Async => DbReplicaType.Async,
            _ => throw new ArgumentOutOfRangeException(nameof(replicaType), replicaType, null)
        };
    }
}
