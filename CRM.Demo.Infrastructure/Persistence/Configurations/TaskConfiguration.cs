using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Demo.Domain.Tasks.ValueObjects;
using CRM.Demo.Domain.Customers.Entities;
using CRM.Demo.Domain.Contacts.Entities;
using DomainTask = CRM.Demo.Domain.Tasks.Entities.Task;
using TaskStatus = CRM.Demo.Domain.Tasks.ValueObjects.TaskStatus;

namespace CRM.Demo.Infrastructure.Persistence.Configurations;

/// <summary>
/// Konfiguracja Fluent API dla Task Entity.
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<DomainTask>
{
    public void Configure(EntityTypeBuilder<DomainTask> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        // Podstawowe informacje
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        // Value Objects - używamy konwersji wartości
        builder.Property(t => t.Type)
            .HasConversion(
                type => type.Value,
                value => TaskType.FromString(value))
            .HasColumnName("Type")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Status)
            .HasConversion(
                status => status.Value,
                value => TaskStatus.FromString(value))
            .HasColumnName("Status")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Priority)
            .HasConversion(
                priority => priority.Value,
                value => TaskPriority.FromString(value))
            .HasColumnName("Priority")
            .IsRequired()
            .HasMaxLength(50);

        // Terminy
        builder.Property(t => t.DueDate)
            .IsRequired(false);

        builder.Property(t => t.StartDate)
            .IsRequired(false);

        builder.Property(t => t.CompletedDate)
            .IsRequired(false);

        // Relacje
        builder.Property(t => t.CustomerId)
            .IsRequired(false);
        
        builder.Property(t => t.ContactId)
            .IsRequired(false);

        builder.Property(t => t.AssignedToUserId)
            .IsRequired(false);
        
        // Foreign key constraints z SET NULL
        // Gdy Customer zostanie usunięty, Task.CustomerId zostanie ustawione na NULL
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Gdy Contact zostanie usunięty, Task.ContactId zostanie ustawione na NULL
        builder.HasOne<Contact>()
            .WithMany()
            .HasForeignKey(t => t.ContactId)
            .OnDelete(DeleteBehavior.SetNull);

        // Metadata
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CreatedByUserId)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);

        builder.Ignore(t => t.IsOverdue); // Computed property

        // Indeksy
        builder.HasIndex(t => t.CustomerId)
            .HasDatabaseName("IX_Tasks_CustomerId");

        builder.HasIndex(t => t.ContactId)
            .HasDatabaseName("IX_Tasks_ContactId");

        builder.HasIndex(t => t.AssignedToUserId)
            .HasDatabaseName("IX_Tasks_AssignedToUserId");

        builder.HasIndex(t => t.DueDate)
            .HasDatabaseName("IX_Tasks_DueDate");

        // Ignoruj Domain Events
        builder.Ignore(t => t.DomainEvents);
    }
}
