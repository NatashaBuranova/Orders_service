using Ozon.Route256.Five.OrderService.Controllers.DTO.Clients;

namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

public record OrderResponse
{
    public long Id { get; set; }
    public int CountProduct { get; set; }
    public long TotalSumm { get; set; }
    public long TotalWeight { get; set; }
    public DateTime DateCreate { get; set; }
    public int Status { get; set; }
    public string DeliveryAddress { get; set; } = "";
    public long Telephone { get; set; }
    public ClientResponse Client { get; set; } = new ClientResponse();
}



