using Ozon.Route256.Five.OrderService.Models;

namespace Ozon.Route256.Five.OrderService.Repositories;

public interface IRegionRepository
{
    Task<Region[]> GetAllAsync(CancellationToken token);

    Task<bool> IsExistsAsync(long regionId, CancellationToken token);
}
