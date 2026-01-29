using CRM.Demo.Domain.Common;

namespace CRM.Demo.Application.Common.Interfaces;

/// <summary>
/// Interfejs dla systemu komunikacji asynchronicznej (Kafka).
/// Publikuje Domain Events do innych modułów.
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Publikuje Domain Event.
    /// </summary>
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Publikuje wiele Domain Events.
    /// </summary>
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
