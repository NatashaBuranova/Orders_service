using Ozon.Route256.Five.OrderService.Models;
using Ozon.Route256.Five.OrderService.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

public class OrderResponse
{
    public long Id { get; set; }
    public int CountProduct { get; set; }
    public long TotalSumm { get; set; }
    public long TotalWeight { get; set; }
    public DateTimeOffset DateCreate { get; set; }
    public OrderState Status { get; set; }
    public Adress DeliveryAddress { get; set; } = new Adress();
    public string Telephone { get; set; }
    public Client Client { get; set; } = new Client();
}

public class Client
{
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
}


