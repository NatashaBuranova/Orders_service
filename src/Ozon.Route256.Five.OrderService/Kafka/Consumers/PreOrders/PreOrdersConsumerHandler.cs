using Ozon.Route256.Five.OrderService.DateTimeProvider;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders.DTO;
using Ozon.Route256.Five.OrderService.Models;
using Ozon.Route256.Five.OrderService.Models.Enums;
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

        await AddClientAsync(client, token);
        await _orderRepository.InsertAsync(newOrder, token);

        await _sendNewOrder.SendValidOrder(newOrder, token);
    }

    private async Task<Order> GetNewOrderFromMessageAsync(PreOrderRequest message, CancellationToken token)
    {
        var region = await _regionRepository.FindAsync(message.Customer.Address.Region, token);

        if (region == null)
            throw new Exception($"For customer with id={message.Customer.Id} not found region {message.Customer.Address.Region}");

        return new Order()
        {
            Id = message.Id,
            DateCreate = _dateTimeProvider.CurrentDateTimeOffsetUtc,
            Type = message.Source,
            ClientId = message.Customer.Id,
            CountProduct = message.Goods.Count,
            State = OrderState.Created,
            DeliveryAddress = new Models.Address()
            {
                Region = message.Customer.Address.Region,
                Apartment = message.Customer.Address.Apartment,
                Building = message.Customer.Address.Building,
                City = message.Customer.Address.City,
                Latitude = message.Customer.Address.Latitude,
                Longitude = message.Customer.Address.Longitude,
                Street = message.Customer.Address.Street,
            },
            TotalSumm = message.Goods.Select(x => x.Price).Sum(),
            TotalWeight = message.Goods.Select(x => x.Weight).Sum(),
            RegionId = region.Id,
        };
    }

    private async Task<Models.Client> GetClientAsync(int clientId, CancellationToken token)
    {
        var client = await _clientServices.GetClientAsync(clientId, token);

        return new Models.Client()
        {
            Id = client.Id,
            FirstName = client.FirstName ?? string.Empty,
            LastName = client.LastName ?? string.Empty,
            Telephone = client.Telephone ?? string.Empty
        };
    }

    private async Task AddClientAsync(Models.Client client, CancellationToken token)
    {
        if (!await _clientRepository.IsExistsAsync(client.Id, token))
        {
            await _clientRepository.InsertAsync(client, token);
        }
    }
}

