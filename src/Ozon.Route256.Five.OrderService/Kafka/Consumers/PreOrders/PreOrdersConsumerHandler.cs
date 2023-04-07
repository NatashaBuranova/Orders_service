using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Core.Models.Enums;
using Ozon.Route256.Five.OrderService.DateTimeProvider;
using Ozon.Route256.Five.OrderService.Infrastructure.Kafka.Consumers.BackgroundConsumer;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders.DTO;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Consumers.Kafka.PreOrders;

public class PreOrdersConsumerHandler : IKafkaConsumerHandler<string, PreOrderRequest>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IGetClientServices _clientServices;
    private readonly IRegionRepository _regionRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ISendNewOrder _sendNewOrder;

    public PreOrdersConsumerHandler(IOrderRepository orderRepository,
        IGetClientServices clinetServices,
        IRegionRepository regionRepository,
        IDateTimeProvider dateTimeProvider,
        ISendNewOrder sendNewOrder,
        IClientRepository clientRepository)
    {
        _orderRepository = orderRepository;
        _clientServices = clinetServices;
        _regionRepository = regionRepository;
        _dateTimeProvider = dateTimeProvider;
        _sendNewOrder = sendNewOrder;
        _clientRepository = clientRepository;
    }

    public async Task HandleAsync(string key, PreOrderRequest message, CancellationToken token)
    {
        var newOrder = await GetNewOrderFromMessageAsync(message, token);
        var client = await GetClientAsync(message.Customer.Id, token);

        await AddClientAsync(client, newOrder.RegionId, token);
        await _orderRepository.InsertAsync(newOrder, token);

        await _sendNewOrder.SendValidOrder(newOrder, token);
    }

    private async Task<Order> GetNewOrderFromMessageAsync(PreOrderRequest message, CancellationToken token)
    {
        var region = await _regionRepository.FindAsync(message.Customer.Address.Region, token);

        if (region == null)
            throw new Exception($"For customer with id={message.Customer.Id} not found region {message.Customer.Address.Region}");

        return GetOrder(message, region.Id);
    }

    private Order GetOrder(PreOrderRequest message, long regionId)
    {
        return new Order()
        {
            Id = message.Id,
            DateCreate = _dateTimeProvider.CurrentDateTimeOffsetUtc,
            Type = message.Source,
            ClientId = message.Customer.Id,
            CountProduct = message.Goods.Count,
            State = OrderState.Created,
            DeliveryAddress = GetAddress(message.Customer.Address),
            TotalSumm = message.Goods.Select(x => x.Price).Sum(),
            TotalWeight = message.Goods.Select(x => x.Weight).Sum(),
            RegionId = regionId,
        };
    }

    private static Address GetAddress(PreOrderAddressRequest address)
    {
        return new Address()
        {
            Region = address.Region,
            Apartment = address.Apartment,
            Building = address.Building,
            City = address.City,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            Street = address.Street,
        };
    }

    private async Task<Client> GetClientAsync(int clientId, CancellationToken token)
    {
        var client = await _clientServices.GetClientAsync(clientId, token);

        return new Client()
        {
            Id = client.Id,
            FirstName = client.FirstName ?? string.Empty,
            LastName = client.LastName ?? string.Empty,
            Telephone = client.Telephone ?? string.Empty
        };
    }

    private async Task AddClientAsync(Client client, long regionId, CancellationToken token)
    {
        if (!await _clientRepository.IsExistsAsync(client.Id, token))
        {
            await _clientRepository.InsertAsync(client, regionId, token);
        }
    }
}

