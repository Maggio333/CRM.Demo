using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using CRM.Demo.Domain.Common;
using CRM.Demo.Application.Common.Interfaces;
using CRM.Demo.Infrastructure.Persistence;

namespace CRM.Demo.Infrastructure.Persistence;

/// <summary>
/// Implementacja Unit of Work Pattern.
/// Zarządza transakcjami i publikowaniem Domain Events.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        ApplicationDbContext context,
        IMessageBus messageBus,
        ILogger<UnitOfWork> logger)
    {
        _context = context;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Zbierz Domain Events z wszystkich Entities w ChangeTracker
        var domainEvents = GetDomainEvents();

        // 2. Zapisz zmiany do bazy danych
        var result = await _context.SaveChangesAsync(cancellationToken);

        // 3. Publikuj Domain Events PO udanym zapisie
        await PublishDomainEventsAsync(domainEvents, cancellationToken);

        return result;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            _logger.LogWarning("Transaction already started");
            return;
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _logger.LogInformation("Transaction started");
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            _logger.LogWarning("No transaction to commit");
            return;
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Transaction committed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing transaction");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            _logger.LogWarning("No transaction to rollback");
            return;
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
            _logger.LogInformation("Transaction rolled back");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back transaction");
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Zbiera wszystkie Domain Events z Entities w ChangeTracker.
    /// </summary>
    private List<IDomainEvent> GetDomainEvents()
    {
        var domainEvents = new List<IDomainEvent>();

        var entries = _context.ChangeTracker
            .Entries<Entity<Guid>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .ToList();

        foreach (var entry in entries)
        {
            domainEvents.AddRange(entry.Entity.DomainEvents);
            entry.Entity.ClearDomainEvents();
        }

        return domainEvents;
    }

    /// <summary>
    /// Publikuje Domain Events przez MessageBus (Kafka).
    /// </summary>
    private async Task PublishDomainEventsAsync(
        List<IDomainEvent> domainEvents,
        CancellationToken cancellationToken)
    {
        if (!domainEvents.Any())
        {
            return;
        }

        _logger.LogInformation(
            "Publishing {Count} domain events",
            domainEvents.Count
        );

        foreach (var domainEvent in domainEvents)
        {
            try
            {
                await _messageBus.PublishAsync(domainEvent, cancellationToken);
                _logger.LogInformation(
                    "Published domain event: {EventType}",
                    domainEvent.GetType().Name
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error publishing domain event: {EventType}",
                    domainEvent.GetType().Name
                );
                // Nie rzucamy wyjątku - publikacja Events nie powinna blokować zapisu
                // W produkcji można użyć Outbox Pattern
            }
        }
    }
}
