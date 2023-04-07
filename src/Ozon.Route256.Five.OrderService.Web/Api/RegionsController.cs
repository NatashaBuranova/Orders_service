using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Web.Api.Helpers;

namespace Ozon.Route256.Five.OrderService.Web.Api;

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
        var regions = RegionHelpers.GetRegionResponse(await _regionRepository.GetAllAsync(token));

        return Ok(regions);
    }
}