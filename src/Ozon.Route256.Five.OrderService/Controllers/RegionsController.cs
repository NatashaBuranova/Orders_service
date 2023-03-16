using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Regions;

namespace Ozon.Route256.Five.OrderService.Controllers;

public class RegionsController : BaseController
{
    public RegionsController() { }

    [HttpGet]
    public async Task<IActionResult> GetRegionsListAsync()
    {
        await Task.Yield();

        return Ok(Enumerable.Empty<List<RegionResponse>>());
    }
}