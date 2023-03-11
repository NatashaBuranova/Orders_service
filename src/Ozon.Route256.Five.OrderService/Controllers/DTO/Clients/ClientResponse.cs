using Ozon.Route256.Five.OrderService.Controllers.DTO.Regions;

namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Clients;

public record ClientResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public AdressResponse DefaultAdress { get; set; }
    public List<AdressResponse> DefaultAdresses { get; set; } = new List<AdressResponse>();
}


