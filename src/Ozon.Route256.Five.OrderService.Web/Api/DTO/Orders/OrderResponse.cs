using Ozon.Route256.Five.OrderService.Core.Models.Enums;

namespace Ozon.Route256.Five.OrderService.Web.Api.DTO.Orders;

public class OrderResponse
{
    public long Id { get; set; }
    public int CountProduct { get; set; }
    public double TotalSumm { get; set; }
    public long TotalWeight { get; set; }
    public DateTimeOffset DateCreate { get; set; }
    public OrderState Status { get; set; }
    public Core.Models.Address? DeliveryAddress { get; set; }
    public string? Telephone { get; set; }
    public ClientName? Client { get; set; }
}




