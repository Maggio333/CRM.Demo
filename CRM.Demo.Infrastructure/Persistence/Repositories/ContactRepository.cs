using Microsoft.EntityFrameworkCore;
using CRM.Demo.Domain.Contacts.Entities;
using CRM.Demo.Application.Contacts.Interfaces;
using CRM.Demo.Infrastructure.Persistence;

namespace CRM.Demo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementacja repozytorium dla Contact.
/// </summary>
public class ContactRepository : Repository<Contact, Guid>, IContactRepository
{
    public ContactRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Contact?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email.Value == email, cancellationToken);
    }

    public async Task<List<Contact>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Contact>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }
}
