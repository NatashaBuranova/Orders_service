using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.GrpsServices;

public class OrdersService : Orders.OrdersBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGetClientServices _clientServices;

    public OrdersService(IOrderRepository orderRepository, IGetClientServices clientServices)
    {
        _orderRepository = orderRepository;
        _clientServices = clientServices;
    }

    public override async Task<OrderByIdResponse> GetOrderById(OrderByIdRequest request, ServerCallContext context)
    {
        var order = await _orderRepository.FindAsync(request.Id, context.CancellationToken);

        if (order == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.Id} not found"));

        var client = await _clientServices.GetClientAsync(order.ClientId, context.CancellationToken);

        return new OrderByIdResponse()
        {
            Id = order.Id,
            DateCreate = order.DateCreate.ToTimestamp(),
            DeliveryAddress = new Address()
            {
                Region = order.DeliveryAddress.Region.Name,
                Street = order.DeliveryAddress.Street,
                Apartment = order.DeliveryAddress.Apartment,
                City = order.DeliveryAddress.City,
                Building = order.DeliveryAddress.Building,
                Latitude = order.DeliveryAddress.Latitude,
                Longitude = order.DeliveryAddress.Longitude
            },
            Client = new Client()
            {
                FirstName = client.FirstName,
                LastName = client.LastName
            },
            CountProduct = order.CountProduct,
            Region = order.DeliveryAddress.Region.Name,
            Status = (int)order.State,
            Telephone = client.MobileNumber,
            TotalSum = order.TotalSumm,
            TotalWeight = order.TotalWeight,
            Type = (int)order.Type
        };
    }
}


