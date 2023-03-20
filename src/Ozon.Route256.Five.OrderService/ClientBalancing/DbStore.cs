namespace Ozon.Route256.Five.OrderService.ClientBalancing;

public class DbStore : IDbStore
{
    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();

    private int _currentIndex = -1;

    public Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints)
    {
        var endpoints = new DbEndpoint[dbEndpoints.Count];

        var i = 0;

        foreach (var endpoint in dbEndpoints)
        {
            endpoints[i++] = endpoint;
        }

        _endpoints = endpoints;

        return Task.CompletedTask;
    }

    public Task<DbEndpoint> GetNextEndpointAsync()
    {
        var endpoints = _endpoints;

        var nextIndex = Interlocked.Increment(ref _currentIndex);

        nextIndex %= endpoints.Length;
        nextIndex = nextIndex >= 0 ? nextIndex : endpoints.Length + nextIndex;

        return Task.FromResult(endpoints[nextIndex]);
    }
}
