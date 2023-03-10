using Ozon.Route256.Five.OrderService.Models;
using System.Collections.Concurrent;

namespace Ozon.Route256.Five.OrderService.Repositories.ImMemoryImp;

public class RegionInMemoryRepository : IRegionRepository
{
    private readonly ConcurrentDictionary<long, Region> _region = new();

    public RegionInMemoryRepository()
    {
        _region[1] = new Region() { Id = 1, Name = "Moscow" };
        _region[2] = new Region() { Id = 2, Name = "StPetersburg" };
        _region[3] = new Region() { Id = 3, Name = "Novosibirsk" };
    }

    public Task<Region[]> GetAllAsync(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<Region[]>(token);

        return Task.FromResult(_region.Values.ToArray());
    }

    public Task<bool> IsExistsAsync(long regionId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<bool>(token);

        return Task.FromResult(_region.ContainsKey(regionId));
    }
}
