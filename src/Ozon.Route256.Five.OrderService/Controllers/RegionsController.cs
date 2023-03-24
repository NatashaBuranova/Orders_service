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
        //var options = new JsonSerializerOptions
        //{
        //    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
        //    WriteIndented = true
        //};
        //var test = new Adress() { Region = "Удмуртия", Apartment = "2", Building = "3", City = "Ижевск", Latitude = 11, Longitude = 123, Street = "удмуртская" };
        //var stringTest = JsonSerializer.Serialize(test, options).ToString();

        var test = await _regionRepository.FindAsync("Удмуртия", token);

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