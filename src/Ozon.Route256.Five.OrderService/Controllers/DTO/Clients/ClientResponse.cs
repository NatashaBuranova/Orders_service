namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Clients;

public record ClientResponse
{
    public long Id { get; set; }
    public string? LastName { get; set; }
    public string? FirstName{ get; set; }
}
