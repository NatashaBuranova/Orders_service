using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Web.Api.DTO.Regions;

namespace Ozon.Route256.Five.OrderService.Web.Api.Helpers;

public static class RegionHelpers
{
    public static List<RegionResponse> GetRegionResponse(Region[] regions)
    {
        return regions.Select(x => new RegionResponse
        {
            Id = x.Id,
            Name = x.Name,
        }).ToList();
    }
}
