using Ozon.Route256.Five.OrderService.Core.Models;
using System.Collections.Concurrent;


namespace Ozon.Route256.Five.OrderService.Repositories.ImMemoryImp;

public class ClientInMemoryRepository : IClientRepository
{
    private readonly ConcurrentDictionary<long, Client> _clients = new();

    public ClientInMemoryRepository() { }

    public Task InsertAsync(Client newClient, long regionId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled(token);

        if (_clients.ContainsKey(newClient.Id))
            throw new Exception($"Client with id {newClient.Id} already exists");

        _clients[newClient.Id] = newClient;

        return Task.CompletedTask;
    }

    public Task<bool> IsExistsAsync(long clientId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<bool>(token);

        return Task.FromResult(_clients.ContainsKey(clientId));
    }
}
