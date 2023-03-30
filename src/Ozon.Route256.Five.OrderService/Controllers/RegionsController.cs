using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Regions;
using Ozon.Route256.Five.OrderService.Repositories;

namespace Ozon.Route256.Five.OrderService.Controllers;

public class RegionsController : BaseController
{
    private readonly IRegionRepository _regionRepository;
    public RegionsController(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetRegionsListAsync(CancellationToken token)
    {
        var regions = (await _regionRepository.GetAllAsync(token))
            .Select(x => new RegionResponse
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToArray();

        return Ok(regions);
    }
}