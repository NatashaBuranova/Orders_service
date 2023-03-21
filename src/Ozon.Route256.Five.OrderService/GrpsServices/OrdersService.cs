using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Five.OrderService.Repositories;

namespace Ozon.Route256.Five.OrderService.GrpsServices;

public class OrdersService : Orders.OrdersBase
{
    private readonly IOrderRepository _orderRepository;

    public OrdersService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public override async Task<OrderByIdResponse> GetOrderById(OrderByIdRequest request, ServerCallContext context)
    {
        var order = await _orderRepository.FindAsync(request.Id, context.CancellationToken);

        if (order == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.Id} not found"));

        return new OrderByIdResponse()
        {
            Id = order.Id,
            DateCreate = order.DateCreate.ToTimestamp(),
            DeliveryAddress = new Address()
            {
                Region = order.DeliveryAddress.Region,
                Street = order.DeliveryAddress.Street,
                Apartment = order.DeliveryAddress.Apartment,
                City = order.DeliveryAddress.City,
                Building = order.DeliveryAddress.Building,
                Latitude = order.DeliveryAddress.Latitude,
                Longitude = order.DeliveryAddress.Longitude
            },
            Client = new Client()
            {
                FirstName = order.Client.FirstName,
                LastName = order.Client.LastName
            },
            CountProduct = order.CountProduct,
            Region = order.DeliveryAddress.Region,
            Status = (int)order.State,
            Telephone = order.Client.Telephone,
            TotalSum = order.TotalSumm,
            TotalWeight = order.TotalWeight,
            Type = (int)order.Type
        };
    }
}


