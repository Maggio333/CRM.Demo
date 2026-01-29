using CRM.Demo.Domain.Notes.Entities;
using CRM.Demo.Application.Common.Interfaces;

namespace CRM.Demo.Application.Notes.Interfaces;

/// <summary>
/// Interfejs repozytorium dla Note.
/// Definiuje operacje specyficzne dla Note.
/// </summary>
public interface INoteRepository : IRepository<Note, Guid>
{
    /// <summary>
    /// Wyszukuje wszystkie Notatki dla danego Customer.
    /// </summary>
    Task<List<Note>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Wyszukuje wszystkie Notatki dla danego Contact.
    /// </summary>
    Task<List<Note>> GetByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Wyszukuje wszystkie Notatki dla danego Task.
    /// </summary>
    Task<List<Note>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Wyszukuje wszystkie Notatki utworzone przez użytkownika.
    /// </summary>
    Task<List<Note>> GetByCreatedByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
