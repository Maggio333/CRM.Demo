using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Customers.ValueObjects;
using CRM.Demo.Domain.Customers.DomainEvents;

namespace CRM.Demo.Domain.Customers.Entities;

public class Customer : Entity<Guid>
{
    // Właściwości biznesowe
    public string CompanyName { get; private set; }
    public string TaxId { get; private set; }  // NIP

    // Value Objects zamiast primitywów
    public Email Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Address? Address { get; private set; }
    public CustomerStatus Status { get; private set; }

    // Relacje (tylko referencje do ID innych agregatów)
    public Guid? AssignedSalesRepId { get; private set; }

    // Metadata
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Prywatny konstruktor - wymusza użycie factory method
    private Customer() { } // Dla EF Core

    // Factory method - jedyny sposób na utworzenie Customer
    public static Customer Create(
        string companyName,
        string taxId,
        Email email,
        PhoneNumber? phoneNumber = null,
        Address? address = null)
    {
        // Walidacja biznesowa
        if (string.IsNullOrWhiteSpace(companyName))
            throw new DomainException("Company name cannot be empty");

        if (string.IsNullOrWhiteSpace(taxId))
            throw new DomainException("Tax ID cannot be empty");

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            CompanyName = companyName,
            TaxId = taxId,
            Email = email,
            PhoneNumber = phoneNumber,
            Address = address,
            Status = CustomerStatus.Prospect, // Domyślny status
            CreatedAt = DateTime.UtcNow
        };

        // Publikujemy Domain Event
        customer.AddDomainEvent(new CustomerCreatedEvent(
            customer.Id,
            customer.CompanyName,
            customer.Email.Value,
            customer.CreatedAt
        ));

        return customer;
    }

    // Metody biznesowe - logika domenowa
    public void UpdateContactInfo(Email email, PhoneNumber? phoneNumber, Address? address)
    {
        if (Status == CustomerStatus.Archived)
            throw new DomainException("Cannot update archived customer");

        var oldEmail = Email;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        UpdatedAt = DateTime.UtcNow;

        // Event jeśli email się zmienił
        if (oldEmail != email)
        {
            AddDomainEvent(new CustomerUpdatedEvent(
                Id,
                "Email changed",
                DateTime.UtcNow
            ));
        }
    }

    public void ChangeStatus(CustomerStatus newStatus)
    {
        if (Status == newStatus)
            return;

        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CustomerStatusChangedEvent(
            Id,
            oldStatus,
            newStatus,
            DateTime.UtcNow
        ));
    }

    public void AssignSalesRep(Guid salesRepId)
    {
        if (AssignedSalesRepId == salesRepId)
            return;

        AssignedSalesRepId = salesRepId;
        UpdatedAt = DateTime.UtcNow;
    }
}
