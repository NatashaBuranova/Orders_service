using Confluent.Kafka;
using Ozon.Route256.Five.OrderService.Kafka.Settings;

namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;

public class BackgroundKafkaConsumer<TKey, TMessage, THandler> : BackgroundService
    where THandler : IKafkaConsumerHandler<TKey, TMessage>
{
    private readonly ILogger<BackgroundKafkaConsumer<TKey, TMessage, THandler>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerConfig _config;
    private readonly IDeserializer<TKey> _keyDeserializer;
    private readonly IDeserializer<TMessage> _messageDeserializer;
    private readonly string _topic;
    private readonly TimeSpan _timeoutForRetry;


    public BackgroundKafkaConsumer(
        IServiceProvider serviceProvider,
        KafkaSettings kafkaSettings,
        ConsumerSettings consumerSettings,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TMessage> messageDeserializer)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<BackgroundKafkaConsumer<TKey, TMessage, THandler>>>();
        _serviceProvider = serviceProvider;
        _keyDeserializer = keyDeserializer;
        _messageDeserializer = messageDeserializer;
        _timeoutForRetry = TimeSpan.FromSeconds(kafkaSettings.TimeoutForRetryInSecond);

        _config = new ConsumerConfig
        {
            GroupId = kafkaSettings.GroupId,
            BootstrapServers = kafkaSettings.BootstrapServers,
            EnableAutoCommit = consumerSettings.AutoCommit
        };

        if (string.IsNullOrWhiteSpace(consumerSettings.Topic))
            throw new Exception("Topic is empty");
        _topic = consumerSettings.Topic;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Consume(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in topic {Topic} during kafka consume", _topic);
                await Task.Delay(_timeoutForRetry, stoppingToken);
            }
        }
    }

    private async Task Consume(CancellationToken token)
    {
        using var consumer = new ConsumerBuilder<TKey, TMessage>(_config)
            .SetValueDeserializer(_messageDeserializer)
            .SetKeyDeserializer(_keyDeserializer)
            .Build();

        consumer.Subscribe(_topic);
        _logger.LogInformation("Success subscribe to {Topic}", _topic);

        while (!token.IsCancellationRequested)
        {
            var consumed = consumer.Consume(token);
            await _serviceProvider.CreateScope().ServiceProvider.
                GetRequiredService<THandler>()
                .HandleAsync(consumed.Message.Key, consumed.Message.Value, token);
            consumer.Commit();
        }
    }
}

