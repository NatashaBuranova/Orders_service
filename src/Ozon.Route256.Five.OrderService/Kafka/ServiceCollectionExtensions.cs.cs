using Confluent.Kafka;
using Ozon.Route256.Five.OrderService.Consumers.Kafka.Consumers;
using Ozon.Route256.Five.OrderService.Consumers.Kafka.OrderEvents;
using Ozon.Route256.Five.OrderService.Consumers.Kafka.PreOrders;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;
using Ozon.Route256.Five.OrderService.Kafka.Producers;
using Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrder;
using Ozon.Route256.Five.OrderService.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Kafka;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaSettings = configuration.GetSection("Kafka").Get<KafkaSettings>();

        services.AddConsumer<string, PreOrderDto, PreOrdersConsumerHandler>(
            configuration,
            ConsumerType.PreOrder,
            kafkaSettings,
            Deserializers.Utf8,
            new KafkaJsonSerializer<PreOrderDto>());

        services.AddConsumer<string, OrderEventDTO, OrderEventsConsumerHandler>(
           configuration,
           ConsumerType.OrderEvents,
           kafkaSettings,
           Deserializers.Utf8,
           new KafkaJsonSerializer<OrderEventDTO>());

        services.AddProducer(configuration, kafkaSettings);

        services.Configure<OrderEventSettings>(configuration.GetSection(OrderEventSettings.Sections));
        services.AddTransient<INewOrderKafkaPublisher, NewOrderKafkaPublisher>();

        return services;
    }
}
