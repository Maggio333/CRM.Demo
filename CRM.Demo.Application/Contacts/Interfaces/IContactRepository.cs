using CRM.Demo.Domain.Contacts.Entities;
using CRM.Demo.Application.Common.Interfaces;

namespace CRM.Demo.Application.Contacts.Interfaces;

/// <summary>
/// Interfejs repozytorium dla Contact.
/// Definiuje operacje specyficzne dla Contact.
/// </summary>
public interface IContactRepository : IRepository<Contact, Guid>
{
    /// <summary>
    /// Wyszukuje Contact po emailu.
    /// </summary>
    Task<Contact?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wyszukuje wszystkie Contacty dla danego Customer.
    /// </summary>
    Task<List<Contact>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Wyszukuje Contacty po imieniu i nazwisku (częściowe dopasowanie).
    /// </summary>
    Task<List<Contact>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
}
