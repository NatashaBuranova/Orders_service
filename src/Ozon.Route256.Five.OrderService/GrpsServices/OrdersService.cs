using Grpc.Core;

namespace Ozon.Route256.Five.OrderService.GrpsServices;

public class OrdersService : Orders.OrdersBase
{
    public override Task<Order> GetOrderById(OrderByIdRequest request, ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.Id} not found"));
    }
}

