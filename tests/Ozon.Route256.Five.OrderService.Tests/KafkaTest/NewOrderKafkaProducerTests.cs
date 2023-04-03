using Microsoft.Extensions.Options;
using Moq;
using Ozon.Route256.Five.OrderService.Kafka.Producers;
using Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrder;

namespace Ozon.Route256.Five.OrderService.Tests.KafkaTest;

public class NewOrderKafkaProducerTests
{
    private readonly Mock<IKafkaProducer> _kafkaProducerMock;
    private readonly Mock<IOptionsSnapshot<OrderEventSettings>> _optionsSnapshotMock;

    public NewOrderKafkaProducerTests()
    {
        _kafkaProducerMock = new Mock<IKafkaProducer>();
        _optionsSnapshotMock = new Mock<IOptionsSnapshot<OrderEventSettings>>();       
    }

    [Fact]
    public async Task PublishToKafka_WithValidRequest_ProducesMessage()
    {
        // Arrange
        var request = new NewOrderRequest(1);
        var topic = "test-topic";
        var cancellationToken = CancellationToken.None;
        _optionsSnapshotMock.Setup(x => x.Value).Returns(new OrderEventSettings { Topic = topic });
        var producer = new NewOrderKafkaProducer(_kafkaProducerMock.Object, _optionsSnapshotMock.Object);

        // Act
        await producer.PublishToKafka(request, cancellationToken);

        // Assert
        _kafkaProducerMock.Verify(x => x.SendMessage(request.OrderId.ToString(), It.IsAny<string>(), topic, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task PublishToKafka_WithEmptyTopic_ThrowsException()
    {
        // Arrange
        var request = new NewOrderRequest(1);
        var cancellationToken = CancellationToken.None;
        _optionsSnapshotMock.Setup(x => x.Value).Returns(new OrderEventSettings { Topic = "" });
        var producer = new NewOrderKafkaProducer(_kafkaProducerMock.Object, _optionsSnapshotMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => producer.PublishToKafka(request, cancellationToken));
    }
}