namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders
{
    public record OrdersInRegionResponse
    {
        public string? RegionName { get; set; }
        public long CountOrders { get; set; }
        public long TotalSumOrders { get; set; }
        public long TotalWeight { get; set; }
        public long CountClients { get; set; }
    }
}
