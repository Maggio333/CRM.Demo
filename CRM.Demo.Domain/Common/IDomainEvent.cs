namespace CRM.Demo.Domain.Common;

/// <summary>
/// Marker interface dla Domain Events.
/// Wszystkie Domain Events muszą implementować ten interfejs.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Data i czas wystąpienia zdarzenia.
    /// </summary>
    DateTime OccurredOn { get; }
}
