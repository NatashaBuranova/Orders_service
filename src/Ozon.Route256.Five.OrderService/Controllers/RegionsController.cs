using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Controllers.DTO.Regions;
using Ozon.Route256.Five.OrderService.Models;
using Ozon.Route256.Five.OrderService.Repositories;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

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
        //var test = new Adress() { Region = "��������", Apartment = "2", Building = "3", City = "������", Latitude = 11, Longitude = 123, Street = "����������" };
        //var stringTest = JsonSerializer.Serialize(test, options).ToString();

        var test = await _regionRepository.FindAsync("��������", token);

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