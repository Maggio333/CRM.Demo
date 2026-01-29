using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Demo.Domain.Notes.Entities;
using CRM.Demo.Domain.Notes.ValueObjects;
using CRM.Demo.Domain.Customers.Entities;
using CRM.Demo.Domain.Contacts.Entities;
using DomainTask = CRM.Demo.Domain.Tasks.Entities.Task;

namespace CRM.Demo.Infrastructure.Persistence.Configurations;

/// <summary>
/// Konfiguracja Fluent API dla Note Entity.
/// </summary>
public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("Notes");

        builder.HasKey(n => n.Id);

        // Podstawowe informacje
        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(n => n.Title)
            .HasMaxLength(200);

        // Value Objects - używamy konwersji wartości
        builder.Property(n => n.Type)
            .HasConversion(
                type => type.Value,
                value => NoteType.FromString(value))
            .HasColumnName("Type")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(n => n.Category)
            .HasConversion(
                category => category != null ? category.Value : null,
                value => value != null ? NoteCategory.FromString(value) : null!)
            .HasColumnName("Category")
            .HasMaxLength(50);

        // Relacje
        builder.Property(n => n.CustomerId)
            .IsRequired(false);

        builder.Property(n => n.ContactId)
            .IsRequired(false);

        builder.Property(n => n.TaskId)
            .IsRequired(false);
        
        // Foreign key constraints z SET NULL
        // Gdy Customer zostanie usunięty, Note.CustomerId zostanie ustawione na NULL
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(n => n.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Gdy Contact zostanie usunięty, Note.ContactId zostanie ustawione na NULL
        builder.HasOne<Contact>()
            .WithMany()
            .HasForeignKey(n => n.ContactId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Gdy Task zostanie usunięty, Note.TaskId zostanie ustawione na NULL
        builder.HasOne<DomainTask>()
            .WithMany()
            .HasForeignKey(n => n.TaskId)
            .OnDelete(DeleteBehavior.SetNull);

        // NoteAttachment jako owned collection
        builder.OwnsMany(n => n.Attachments, attachment =>
        {
            attachment.ToTable("NoteAttachments");

            attachment.WithOwner()
                .HasForeignKey("NoteId");

            attachment.Property<Guid>("Id")
                .ValueGeneratedOnAdd();

            attachment.HasKey("Id");

            attachment.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            attachment.Property(a => a.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            attachment.Property(a => a.FileSize)
                .IsRequired();

            attachment.Property(a => a.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            attachment.Property(a => a.UploadedAt)
                .IsRequired();
        });

        // Metadata
        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.CreatedByUserId)
            .IsRequired();

        builder.Property(n => n.UpdatedAt)
            .IsRequired(false);

        builder.Property(n => n.UpdatedByUserId)
            .IsRequired(false);

        // Indeksy
        builder.HasIndex(n => n.CustomerId)
            .HasDatabaseName("IX_Notes_CustomerId");

        builder.HasIndex(n => n.ContactId)
            .HasDatabaseName("IX_Notes_ContactId");

        builder.HasIndex(n => n.TaskId)
            .HasDatabaseName("IX_Notes_TaskId");

        // Ignoruj Domain Events
        builder.Ignore(n => n.DomainEvents);
    }
}
