using Ozon.Route256.Five.OrderService.DateTimeProvider;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;
using Ozon.Route256.Five.OrderService.Models;
using Ozon.Route256.Five.OrderService.Models.Enums;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Consumers.Kafka.PreOrders;

public class PreOrdersConsumerHandler : IKafkaConsumerHandler<string, PreOrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGetClientServices _clientServices;
    private readonly IRegionRepository _regionRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ISendNewOrder _sendNewOrder;

    public PreOrdersConsumerHandler(IOrderRepository orderRepository,
        IGetClientServices clinetServices,
        IRegionRepository regionRepository,
        IDateTimeProvider dateTimeProvider,
        ISendNewOrder sendNewOrder)
    {
        _orderRepository = orderRepository;
        _clientServices = clinetServices;
        _regionRepository = regionRepository;
        _dateTimeProvider = dateTimeProvider;
        _sendNewOrder = sendNewOrder;
    }

    public async Task Handle(string key, PreOrderDto message, CancellationToken token)
    {
        var newOrder = await GetNewOrderFromMessageAsync(message, token);

        await _orderRepository.InsertAsync(newOrder, token);

        await _sendNewOrder.SendValidOrder(newOrder, token);
    }

    private async Task<Order> GetNewOrderFromMessageAsync(PreOrderDto message, CancellationToken token)
    {
        var client = await _clientServices.GetClientAsync(message.Customer.Id, token);
        var region = await _regionRepository.FindAsync(message.Customer.Address.Region, token);

        return new Order()
        {
            Id = message.Id,
            DateCreate = _dateTimeProvider.CurrentDateTimeOffsetUtc,
            Type = message.Source,
            ClientId = client.Id,
            Client = new Models.Client()
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Telephone = client.Telephone
            },
            Goods = message.Goods.Select(x => new Good()
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Quantity = x.Quantity,
                Weight = x.Weight,
            })
            .ToList(),
            CountProduct = message.Goods.Count,
            State = OrderState.Created,
            DeliveryAddress = new Adress()
            {
                Region = region,
                RegionId = region.Id,
                Apartment = message.Customer.Address.Apartment,
                Building = message.Customer.Address.Building,
                City = message.Customer.Address.City,
                Latitude = message.Customer.Address.Latitude,
                Longitude = message.Customer.Address.Longitude,
                Street = message.Customer.Address.Street,
            },
            TotalSumm = message.Goods.Select(x => x.Price).Sum(),
            TotalWeight = message.Goods.Select(x => x.Weight).Sum(),
        };
    }
}

