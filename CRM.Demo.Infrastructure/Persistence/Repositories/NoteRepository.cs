using Microsoft.EntityFrameworkCore;
using CRM.Demo.Domain.Notes.Entities;
using CRM.Demo.Application.Notes.Interfaces;
using CRM.Demo.Infrastructure.Persistence;

namespace CRM.Demo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementacja repozytorium dla Note.
/// </summary>
public class NoteRepository : Repository<Note, Guid>, INoteRepository
{
    public NoteRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<List<Note>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Note>> GetByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.ContactId == contactId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Note>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.TaskId == taskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Note>> GetByCreatedByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.CreatedByUserId == userId)
            .ToListAsync(cancellationToken);
    }
}
