namespace Ozon.Route256.Five.OrderService.Infrastructure.ClientBalancing;

public record DbEndpoint(
    string HostAndPort,
    DbReplicaType DbReplica,
    int[] Buckets);

