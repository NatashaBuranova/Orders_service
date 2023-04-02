namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders;

public class OrdersInRegionResponse
{
    public string? RegionName { get; set; }
    public int CountOrders { get; set; }
    public long TotalSumOrders { get; set; }
    public long TotalWeight { get; set; }
    public long CountClients { get; set; }
}
