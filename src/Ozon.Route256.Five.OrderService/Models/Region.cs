namespace Ozon.Route256.Five.OrderService.Models;

public class Region
{
    public long Id { get; set; }
    public string Name { get; set; }
    public List<Address> Addresses { get; set; } = new();
}
