using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using System.Text.Json;
using CRM.Demo.Domain.Common;
using CRM.Demo.Application.Common.Interfaces;

namespace CRM.Demo.Infrastructure.Messaging;

/// <summary>
/// Implementacja MessageBus używająca Apache Kafka.
/// Publikuje Domain Events do tematów Kafka.
/// </summary>
public class MessageBus : IMessageBus, IDisposable
{
    private readonly ILogger<MessageBus> _logger;
    private readonly IProducer<Null, string> _producer;
    private readonly string _defaultTopic;

    public MessageBus(ILogger<MessageBus> logger, IConfiguration configuration)
    {
        _logger = logger;

        // Konfiguracja Kafka Producer
        var kafkaConfig = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            // Acknowledgment - czekamy na potwierdzenie od wszystkich replik
            Acks = Acks.All,
            // Retry policy
            RetryBackoffMs = 100,
            MessageSendMaxRetries = 3,
            // Idempotent producer - zapewnia dokładnie raz delivery
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<Null, string>(kafkaConfig)
            .SetErrorHandler((producer, error) =>
            {
                _logger.LogError(
                    "Kafka Producer Error: {ErrorCode} - {ErrorMessage}",
                    error.Code,
                    error.Reason
                );
            })
            .Build();

        _defaultTopic = configuration["Kafka:DefaultTopic"] ?? "crm-domain-events";

        _logger.LogInformation(
            "Kafka MessageBus initialized. BootstrapServers: {BootstrapServers}, DefaultTopic: {DefaultTopic}",
            kafkaConfig.BootstrapServers,
            _defaultTopic
        );
    }

    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            // Określ topic na podstawie typu eventu
            var topic = GetTopicForEvent(domainEvent);

            // Serializuj Domain Event do JSON
            var eventJson = JsonSerializer.Serialize(domainEvent, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Publikuj do Kafka
            var message = new Message<Null, string>
            {
                Value = eventJson,
                Headers = new Headers
                {
                    { "EventType", System.Text.Encoding.UTF8.GetBytes(domainEvent.GetType().Name) },
                    { "OccurredOn", System.Text.Encoding.UTF8.GetBytes(domainEvent.OccurredOn.ToString("O")) }
                }
            };

            var deliveryResult = await _producer.ProduceAsync(topic, message, cancellationToken);

            _logger.LogInformation(
                "Published domain event: {EventType} to topic {Topic} at partition {Partition} offset {Offset}",
                domainEvent.GetType().Name,
                deliveryResult.Topic,
                deliveryResult.Partition,
                deliveryResult.Offset
            );
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(
                ex,
                "Error publishing domain event {EventType} to Kafka: {Error}",
                domainEvent.GetType().Name,
                ex.Error.Reason
            );
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error publishing domain event {EventType}",
                domainEvent.GetType().Name
            );
            throw;
        }
    }

    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        var events = domainEvents.ToList();

        _logger.LogInformation(
            "Publishing {Count} domain events to Kafka",
            events.Count
        );

        // Publikuj wszystkie events równolegle
        var tasks = events.Select(evt => PublishAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Określa topic Kafka na podstawie typu Domain Event.
    /// Można rozszerzyć o bardziej zaawansowaną logikę routing.
    /// </summary>
    private string GetTopicForEvent(IDomainEvent domainEvent)
    {
        var eventType = domainEvent.GetType().Name;

        // Routing na podstawie typu eventu
        // Przykład: CustomerCreatedEvent -> customers-events
        if (eventType.Contains("Customer"))
            return "customers-events";

        if (eventType.Contains("Contact"))
            return "contacts-events";

        if (eventType.Contains("Task"))
            return "tasks-events";

        if (eventType.Contains("Note"))
            return "notes-events";

        // Domyślny topic
        return _defaultTopic;
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}
