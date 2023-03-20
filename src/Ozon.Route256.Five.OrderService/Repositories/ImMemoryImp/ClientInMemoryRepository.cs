using System.Collections.Concurrent;

namespace Ozon.Route256.Five.OrderService.Repositories.ImMemoryImp;

public class ClientInMemoryRepository : IClientRepository
{
    private readonly ConcurrentDictionary<long, Client> _clients = new();

    public ClientInMemoryRepository() { }

    public Task<bool> IsExistsAsync(long clientId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<bool>(token);

        return Task.FromResult(_clients.ContainsKey(clientId));
    }
}
