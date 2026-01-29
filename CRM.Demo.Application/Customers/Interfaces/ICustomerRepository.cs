using CRM.Demo.Domain.Customers.Entities;
using CRM.Demo.Application.Common.Interfaces;

namespace CRM.Demo.Application.Customers.Interfaces;

/// <summary>
/// Interfejs repozytorium dla Customer.
/// Definiuje operacje specyficzne dla Customer.
/// </summary>
public interface ICustomerRepository : IRepository<Customer, Guid>
{
    /// <summary>
    /// Wyszukuje Customer po emailu.
    /// </summary>
    Task<Customer?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Wyszukuje Customer po TaxId (NIP).
    /// </summary>
    Task<Customer?> FindByTaxIdAsync(string taxId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Wyszukuje Customer po nazwie firmy (częściowe dopasowanie).
    /// </summary>
    Task<List<Customer>> SearchByCompanyNameAsync(string searchTerm, CancellationToken cancellationToken = default);
}
