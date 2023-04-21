using Moq;
using Ozon.Route256.Five.OrderService.Consumers.Kafka.PreOrders;
using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Core.Models.Enums;
using Ozon.Route256.Five.OrderService.DateTimeProvider;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders.DTO;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Services;
using Ozon.Route256.Five.OrderService.Web.Api.DTO.Clients;

namespace Ozon.Route256.Five.OrderService.Tests.KafkaTest;

public class PreOrdersConsumerHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IGetClientServices> _clientServicesMock = new();
    private readonly Mock<IRegionRepository> _regionRepositoryMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<ISendNewOrder> _sendNewOrderMock = new();
    private readonly Mock<IClientRepository> _clientRepositoryMock = new();

    private readonly PreOrdersConsumerHandler _consumerHandler;

    public PreOrdersConsumerHandlerTests()
    {
        _consumerHandler = new PreOrdersConsumerHandler(
            _orderRepositoryMock.Object,
            _clientServicesMock.Object,
            _regionRepositoryMock.Object,
            _dateTimeProviderMock.Object,
            _sendNewOrderMock.Object,
            _clientRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ClientNotExistInDb_AddNewOrderAndNewClient()
    {
        // Arrange
        var address = new PreOrderAddressRequest("Удмуртия", "Ижевск", "Холмогорова", "345", "23", 23, 34);
        var customer = new PreOrderCustomerRequest(1, address);
        var goods = new List<PreOrderGoodsRequest>() { new PreOrderGoodsRequest(1, "Товар", 12, 1200, 12) };
        var message = new PreOrderRequest(1, OrderType.Web, customer, goods);

        _regionRepositoryMock.Setup(x => x.FindAsync(message.Customer.Address.Region, default))
            .ReturnsAsync(new Region() { Id = 1, Name = "Удмуртия" });

        _clientServicesMock.Setup(x => x.GetClientAsync(message.Customer.Id, default))
            .ReturnsAsync(new ClientResponse() { Id = 1, FirstName = "Иван", LastName = "Иванович" });

        _clientRepositoryMock.Setup(x => x.IsExistsAsync(customer.Id, default))
            .ReturnsAsync(false);

        // Act
        await _consumerHandler.HandleAsync(null, message, default);

        // Assert
        _orderRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<Order>(), default), Times.Once);
        _clientRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<Client>(), It.IsAny<long>(), default), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ClientExistInDb_AddNewOrder()
    {
        // Arrange
        var address = new PreOrderAddressRequest("Удмуртия", "Ижевск", "Холмогорова", "345", "23", 23, 34);
        var customer = new PreOrderCustomerRequest(1, address);
        var goods = new List<PreOrderGoodsRequest>() { new PreOrderGoodsRequest(1, "Товар", 12, 1200, 12) };
        var message = new PreOrderRequest(1, OrderType.Web, customer, goods);

        _regionRepositoryMock.Setup(x => x.FindAsync(message.Customer.Address.Region, default))
            .ReturnsAsync(new Region() { Id = 1, Name = "Удмуртия" });

        _clientServicesMock.Setup(x => x.GetClientAsync(message.Customer.Id, default))
            .ReturnsAsync(new ClientResponse() { Id = 1, FirstName = "Иван", LastName = "Иванович" });

        _clientRepositoryMock.Setup(x => x.IsExistsAsync(customer.Id, default))
            .ReturnsAsync(true);

        // Act
        await _consumerHandler.HandleAsync(null, message, default);

        // Assert
        _orderRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<Order>(), default), Times.Once);
        _clientRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<Client>(), It.IsAny<long>(), default), Times.Never);
    }
}
