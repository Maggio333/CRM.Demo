using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Demo.Domain.Contacts.Entities;
using CRM.Demo.Domain.Contacts.ValueObjects;
using CRM.Demo.Domain.Customers.Entities;

namespace CRM.Demo.Infrastructure.Persistence.Configurations;

/// <summary>
/// Konfiguracja Fluent API dla Contact Entity.
/// </summary>
public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);

        // Podstawowe dane
        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Ignore(c => c.FullName); // Computed property

        // Email jako owned type
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        // PhoneNumber jako owned type (nullable)
        builder.OwnsOne(c => c.PhoneNumber, phone =>
        {
            phone.Property(p => p.CountryCode)
                .HasColumnName("PhoneCountryCode")
                .HasMaxLength(5);

            phone.Property(p => p.Number)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20);

            phone.Ignore(p => p.FullNumber);
        });

        builder.Property(c => c.JobTitle)
            .HasMaxLength(100);

        builder.Property(c => c.Department)
            .HasMaxLength(100);

        // Value Objects - używamy konwersji wartości (Value Objects mają prywatne konstruktory)
        builder.Property(c => c.Type)
            .HasConversion(
                type => type.Value,
                value => ContactType.FromString(value))
            .HasColumnName("Type")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .HasConversion(
                status => status.Value,
                value => ContactStatus.FromString(value))
            .HasColumnName("Status")
            .IsRequired()
            .HasMaxLength(50);

        // Role jako string z konwersją
        builder.Property(c => c.Role)
            .HasConversion(
                role => role != null ? role.Value : null,
                value => value != null ? ContactRole.FromString(value) : null!)
            .HasColumnName("Role")
            .HasMaxLength(50);

        // Relacje
        builder.Property(c => c.CustomerId)
            .IsRequired(false);
        
        // Foreign key constraint z SET NULL - gdy Customer zostanie usunięty, CustomerId w Contacts zostanie ustawione na NULL
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Metadata
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);

        // Indeksy - dla owned type Email nie możemy użyć HasIndex bezpośrednio
        // Indeks będzie utworzony w migracji ręcznie lub przez unique constraint

        // Ignoruj Domain Events
        builder.Ignore(c => c.DomainEvents);
    }
}
