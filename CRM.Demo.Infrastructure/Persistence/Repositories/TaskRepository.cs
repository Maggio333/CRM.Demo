using Microsoft.EntityFrameworkCore;
using CRM.Demo.Domain.Tasks.Entities;
using CRM.Demo.Application.Tasks.Interfaces;
using CRM.Demo.Infrastructure.Persistence;
using DomainTask = CRM.Demo.Domain.Tasks.Entities.Task;

namespace CRM.Demo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementacja repozytorium dla Task.
/// </summary>
public class TaskRepository : Repository<DomainTask, Guid>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<List<DomainTask>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DomainTask>> GetByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ContactId == contactId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DomainTask>> GetByAssignedUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.AssignedToUserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DomainTask>> GetOverdueTasksAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(t => t.DueDate.HasValue &&
                       t.DueDate.Value < now &&
                       t.Status.Value != "Completed")
            .ToListAsync(cancellationToken);
    }
}
