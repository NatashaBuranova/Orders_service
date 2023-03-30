using Ozon.Route256.Five.OrderService.Models;
using System.Collections.Concurrent;

namespace Ozon.Route256.Five.OrderService.Repositories.ImMemoryImp;

public class RegionInMemoryRepository : IRegionRepository
{
    private readonly ConcurrentDictionary<long, Region> _region = new();

    public RegionInMemoryRepository()
    {
        _region[1] = new Region() { Id = 1, Name = "Moscow", StockLatitude = 55.75, StockLongitude = 37.61 };
        _region[2] = new Region() { Id = 2, Name = "StPetersburg", StockLatitude = 59.93, StockLongitude = 30.31 };
        _region[3] = new Region() { Id = 3, Name = "Novosibirsk", StockLatitude = 55.04, StockLongitude = 82.93 };
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

    public Task<Region?> FindAsync(string regionName, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return Task.FromCanceled<Region?>(token);

        var result = _region.Where(x => x.Value.Name == regionName).FirstOrDefault().Value;

        return Task.FromResult(result);
    }
}
