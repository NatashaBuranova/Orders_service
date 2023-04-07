using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.OrdersGrpsService;
using Ozon.Route256.Five.OrderService.Repositories;
using Address = Ozon.Route256.Five.OrderService.OrdersGrpsService.Address;
using Client = Ozon.Route256.Five.OrderService.OrdersGrpsService.Client;

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

        return GetOrderByIdResponse(order);
    }

    private OrderByIdResponse GetOrderByIdResponse(Order order)
    {
        return new OrderByIdResponse()
        {
            Id = order.Id,
            DateCreate = order.DateCreate.ToTimestamp(),
            DeliveryAddress = order.DeliveryAddress != null ? GetAddress(order.DeliveryAddress) : null,
            Client = order.Client != null ? GetClient(order.Client) : null,
            CountProduct = order.CountProduct,
            Region = order.DeliveryAddress?.Region,
            Status = (int)order.State,
            Telephone = order.Client?.Telephone,
            TotalSum = order.TotalSumm,
            TotalWeight = order.TotalWeight,
            Type = (int)order.Type
        };
    }

    private static Address GetAddress(Core.Models.Address address)
    {
        return new Address()
        {
            Region = address.Region,
            Street = address.Street,
            Apartment = address.Apartment,
            City = address.City,
            Building = address.Building,
            Latitude = address.Latitude,
            Longitude = address.Longitude
        };
    }

    private static Client GetClient(Core.Models.Client client)
    {
        return new Client()
        {
            FirstName = client.FirstName,
            LastName = client.LastName
        };
    }
}


