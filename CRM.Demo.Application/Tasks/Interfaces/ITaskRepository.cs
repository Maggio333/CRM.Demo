using CRM.Demo.Domain.Tasks.Entities;
using CRM.Demo.Application.Common.Interfaces;
using DomainTask = CRM.Demo.Domain.Tasks.Entities.Task;

namespace CRM.Demo.Application.Tasks.Interfaces;

/// <summary>
/// Interfejs repozytorium dla Task.
/// Definiuje operacje specyficzne dla Task.
/// </summary>
public interface ITaskRepository : IRepository<DomainTask, Guid>
{
    /// <summary>
    /// Wyszukuje wszystkie Taski dla danego Customer.
    /// </summary>
    Task<List<DomainTask>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Wyszukuje wszystkie Taski dla danego Contact.
    /// </summary>
    Task<List<DomainTask>> GetByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Wyszukuje wszystkie Taski przypisane do użytkownika.
    /// </summary>
    Task<List<DomainTask>> GetByAssignedUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Wyszukuje przeterminowane Taski.
    /// </summary>
    Task<List<DomainTask>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
}
