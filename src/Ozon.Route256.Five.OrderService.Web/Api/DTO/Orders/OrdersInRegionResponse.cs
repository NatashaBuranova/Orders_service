namespace Ozon.Route256.Five.OrderService.Web.Api.DTO.Orders;

public class OrdersInRegionResponse
{
    public string? RegionName { get; set; }
    public int CountOrders { get; set; }
    public double TotalSumOrders { get; set; }
    public long TotalWeight { get; set; }
    public long CountClients { get; set; }
}
