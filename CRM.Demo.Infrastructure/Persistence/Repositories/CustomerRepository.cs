using Microsoft.EntityFrameworkCore;
using CRM.Demo.Domain.Customers.Entities;
using CRM.Demo.Application.Customers.Interfaces;
using CRM.Demo.Infrastructure.Persistence;

namespace CRM.Demo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementacja repozytorium dla Customer.
/// </summary>
public class CustomerRepository : Repository<Customer, Guid>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Customer?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email.Value == email, cancellationToken);
    }

    public async Task<Customer?> FindByTaxIdAsync(string taxId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.TaxId == taxId, cancellationToken);
    }

    public async Task<List<Customer>> SearchByCompanyNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.CompanyName.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }
}
