using Ozon.Route256.Five.OrderService.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Models;

public class Order
{
    public long Id { get; set; }
    public int CountProduct { get; set; }
    public double TotalSumm { get; set; }
    public long TotalWeight { get; set; }
    public DateTimeOffset DateCreate { get; set; }
    public DateTimeOffset DateUpdate { get; set; } 
    public OrderState State { get; set; }
    public OrderType Type { get; set; }
    public Address DeliveryAddress { get; set; } 
    public Client Client { get; set; }
    public int ClientId { get; set; }
    public Region Region { get; set; }
    public long RegionId { get; set; }
}
