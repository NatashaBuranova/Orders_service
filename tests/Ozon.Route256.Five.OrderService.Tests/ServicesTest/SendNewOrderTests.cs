using Moq;
using Ozon.Route256.Five.OrderService.Core.Models;
using Ozon.Route256.Five.OrderService.Core.Services;
using Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrder;
using Ozon.Route256.Five.OrderService.Services;

namespace Ozon.Route256.Five.OrderService.Tests.ServicesTest;

public class SendNewOrderTests
{
    private readonly Mock<INewOrderKafkaProducer> _mockProducer;
    private readonly Mock<IValidateOrderServices> _mockValidateOrderServices;
    private readonly SendNewOrder _sendNewOrder;

    public SendNewOrderTests()
    {
        _mockProducer = new Mock<INewOrderKafkaProducer>();
        _mockValidateOrderServices = new Mock<IValidateOrderServices>();
        _sendNewOrder = new SendNewOrder(_mockProducer.Object, _mockValidateOrderServices.Object);
    }

    [Fact]
    public async Task SendValidOrder_ValidOrder_SendsToKafka()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            DeliveryAddress = new Address { Latitude = 43.7384, Longitude = 7.4246 },
            Region = new Region { StockLatitude = 43.7102, StockLongitude = 7.4246 }
        };
        var token = new CancellationToken();
        _mockValidateOrderServices.Setup(x => x.IsOrderValid(order)).Returns(true);

        // Act
        await _sendNewOrder.SendValidOrder(order, token);

        // Assert
        _mockProducer.Verify(p => p.PublishToKafka(It.IsAny<NewOrderRequest>(), token), Times.Once);
    }

    [Fact]
    public async Task SendValidOrder_InvalidOrder_DoesNotSendsToKafka()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            DeliveryAddress = new Address { Latitude = 55.7558, Longitude = 37.6173 },
            Region = new Region { StockLatitude = 59.9343, StockLongitude = 30.3351 }
        };
        var token = new CancellationToken();
        _mockValidateOrderServices.Setup(x => x.IsOrderValid(order)).Returns(false);

        // Act
        await _sendNewOrder.SendValidOrder(order, token);

        // Assert
        _mockProducer.Verify(p => p.PublishToKafka(It.IsAny<NewOrderRequest>(), token), Times.Never);
    }
}