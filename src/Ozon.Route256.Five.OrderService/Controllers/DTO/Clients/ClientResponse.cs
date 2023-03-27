using Ozon.Route256.Five.OrderService.Controllers.DTO.Regions;

namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Clients;

public class ClientResponse
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public AddressResponse DefaultAddress { get; set; }
    public List<AddressResponse> Addresses { get; set; } = new();
}


