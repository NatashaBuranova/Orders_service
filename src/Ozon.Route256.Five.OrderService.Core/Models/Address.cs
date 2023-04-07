namespace Ozon.Route256.Five.OrderService.Core.Models;

public class Address
{
    public string Region { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string Building { get; set; } = null!;
    public string? Apartment { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
