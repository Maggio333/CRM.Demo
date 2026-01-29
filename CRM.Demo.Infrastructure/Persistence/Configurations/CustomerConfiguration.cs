using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CRM.Demo.Domain.Customers.Entities;
using CRM.Demo.Domain.Customers.ValueObjects;

namespace CRM.Demo.Infrastructure.Persistence.Configurations;

/// <summary>
/// Konfiguracja Fluent API dla Customer Entity.
/// Mapuje Value Objects na kolumny w bazie danych.
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Tabela
        builder.ToTable("Customers");

        // Klucz główny
        builder.HasKey(c => c.Id);

        // Właściwości biznesowe
        builder.Property(c => c.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.TaxId)
            .IsRequired()
            .HasMaxLength(20);

        // Value Objects - mapowanie na kolumny
        // Email jako string (Value Object jest owned type)
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

            phone.Ignore(p => p.FullNumber); // Computed property
        });

        // Address jako owned type (nullable)
        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("AddressStreet")
                .HasMaxLength(200);

            address.Property(a => a.City)
                .HasColumnName("AddressCity")
                .HasMaxLength(100);

            address.Property(a => a.PostalCode)
                .HasColumnName("AddressPostalCode")
                .HasMaxLength(20);

            address.Property(a => a.Country)
                .HasColumnName("AddressCountry")
                .HasMaxLength(100);
        });

        // CustomerStatus jako string z konwersją
        builder.Property(c => c.Status)
            .HasConversion(
                status => status.Value,
                value => CustomerStatus.FromString(value))
            .HasColumnName("Status")
            .IsRequired()
            .HasMaxLength(50);

        // Relacje (tylko ID - nie navigation properties w DDD)
        builder.Property(c => c.AssignedSalesRepId)
            .IsRequired(false);

        // Metadata
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);

        // Indeksy - dla owned type Email nie możemy użyć HasIndex bezpośrednio
        // Unique constraint będzie utworzony w migracji ręcznie
        // Email jest mapowane jako kolumna "Email" w bazie (z OwnsOne)

        builder.HasIndex(c => c.TaxId)
            .IsUnique()
            .HasDatabaseName("IX_Customers_TaxId");

        // Ignoruj Domain Events (nie są w bazie)
        builder.Ignore(c => c.DomainEvents);
    }
}
