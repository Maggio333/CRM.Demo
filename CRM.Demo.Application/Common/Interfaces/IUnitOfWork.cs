namespace CRM.Demo.Application.Common.Interfaces;

/// <summary>
/// Unit of Work Pattern - zarządza transakcjami i publikowaniem Domain Events.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Zapisuje wszystkie zmiany do bazy danych i publikuje Domain Events.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rozpoczyna transakcję.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Zatwierdza transakcję.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cofa transakcję.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
