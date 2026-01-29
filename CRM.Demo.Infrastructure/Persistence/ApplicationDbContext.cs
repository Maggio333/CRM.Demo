using Microsoft.EntityFrameworkCore;
using CRM.Demo.Domain.Customers.Entities;
using CRM.Demo.Domain.Contacts.Entities;
using CRM.Demo.Domain.Notes.Entities;
using DomainTask = CRM.Demo.Domain.Tasks.Entities.Task;

namespace CRM.Demo.Infrastructure.Persistence;

/// <summary>
/// DbContext dla aplikacji CRM.
/// Konfiguruje połączenie z PostgreSQL i mapowania Entity.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets dla wszystkich Entities
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<DomainTask> Tasks { get; set; } = null!;
    public DbSet<Note> Notes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Zastosuj wszystkie konfiguracje z assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
