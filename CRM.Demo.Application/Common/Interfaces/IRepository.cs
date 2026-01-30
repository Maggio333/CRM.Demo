using CRM.Demo.Domain.Common;

namespace CRM.Demo.Application.Common.Interfaces;

/// <summary>
/// Interfejs bazowy dla repozytoriów.
/// Definiuje podstawowe operacje CRUD.
/// </summary>
/// <typeparam name="TEntity">Typ encji</typeparam>
/// <typeparam name="TId">Typ ID encji</typeparam>
public interface IRepository<TEntity, TId> where TEntity : Entity<TId>
{
    /// <summary>
    /// Pobiera encję po ID.
    /// </summary>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera wszystkie encje.
    /// </summary>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Dodaje nową encję.
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje istniejącą encję.
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Usuwa encję.
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sprawdza czy encja istnieje.
    /// </summary>
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
}
