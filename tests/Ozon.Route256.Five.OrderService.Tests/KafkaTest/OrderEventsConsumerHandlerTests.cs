using Moq;
using Ozon.Route256.Five.OrderService.Consumers.Kafka.OrderEvents;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;
using Ozon.Route256.Five.OrderService.Models.Enums;
using Ozon.Route256.Five.OrderService.Repositories;

namespace Ozon.Route256.Five.OrderService.Tests.KafkaTest;

public class OrderEventsConsumerHandlerTests
{
    private Mock<IOrderRepository> _orderRepositoryMock;
    private OrderEventsConsumerHandler _handler;

    public OrderEventsConsumerHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _handler = new OrderEventsConsumerHandler(_orderRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_OrderNotFound_ThrowsException()
    {
        // Arrange
        const long NOT_EXIST_ORDER = 99;
        var message = new OrderEventRequest(NOT_EXIST_ORDER, DateTimeOffset.UtcNow, "SentToCustomer");
        _orderRepositoryMock.Setup(x => x.FindAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync((Models.Order)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(null, message, CancellationToken.None));
        _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Models.Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_OrderFound_UpdatesOrder()
    {
        // Arrange
        var message = new OrderEventRequest(1, DateTimeOffset.UtcNow, "SentToCustomer"); ;
        var order = new Models.Order { Id = 1, State = OrderState.Created, DateUpdate = DateTime.UtcNow };
        _orderRepositoryMock.Setup(x => x.FindAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        // Act
        await _handler.HandleAsync(null, message, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Models.Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
