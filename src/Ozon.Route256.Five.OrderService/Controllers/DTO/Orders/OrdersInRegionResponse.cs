namespace Ozon.Route256.Five.OrderService.Controllers.DTO.Orders
{
    public record OrdersInRegionResponse(long RegionId, List<OrderResponse> Orders);
}
