using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Contacts.ValueObjects;
using CRM.Demo.Domain.Contacts.DomainEvents;

namespace CRM.Demo.Domain.Contacts.Entities;

public class Contact : Entity<Guid>
{
    // Podstawowe dane
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string FullName => $"{FirstName} {LastName}";

    // Kontakt
    public Email Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public string? JobTitle { get; private set; }
    public string? Department { get; private set; }

    // Value Objects
    public ContactType Type { get; private set; }
    public ContactStatus Status { get; private set; }
    public ContactRole? Role { get; private set; }

    // Relacja do Customer (tylko ID - referencja do innego agregatu)
    public Guid? CustomerId { get; private set; }

    // Metadata
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Prywatny konstruktor
    private Contact() { }

    // Factory method
    public static Contact Create(
        string firstName,
        string lastName,
        Email email,
        ContactType type,
        PhoneNumber? phoneNumber = null,
        string? jobTitle = null,
        string? department = null,
        Guid? customerId = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");

        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Type = type,
            PhoneNumber = phoneNumber,
            JobTitle = jobTitle,
            Department = department,
            CustomerId = customerId,
            Status = ContactStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        contact.AddDomainEvent(new ContactCreatedEvent(
            contact.Id,
            contact.FullName,
            contact.Email.Value,
            contact.CreatedAt,
            contact.CustomerId
        ));

        return contact;
    }

    // Metody biznesowe
    public void UpdateContactInfo(
        Email email,
        PhoneNumber? phoneNumber,
        string? jobTitle,
        string? department)
    {
        if (Status == ContactStatus.Archived)
            throw new DomainException("Cannot update archived contact");

        Email = email;
        PhoneNumber = phoneNumber;
        JobTitle = jobTitle;
        Department = department;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContactUpdatedEvent(
            Id,
            "Contact info updated",
            DateTime.UtcNow
        ));
    }

    public void AssignToCustomer(Guid customerId)
    {
        if (CustomerId == customerId)
            return;

        var oldCustomerId = CustomerId;
        CustomerId = customerId;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContactAssignedToCustomerEvent(
            Id,
            oldCustomerId,
            customerId,
            DateTime.UtcNow
        ));
    }

    public void ChangeStatus(ContactStatus newStatus)
    {
        if (Status == newStatus)
            return;

        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContactStatusChangedEvent(
            Id,
            oldStatus,
            newStatus,
            DateTime.UtcNow
        ));
    }

    public void AssignRole(ContactRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    // Metoda do pełnej aktualizacji
    public void Update(
        string firstName,
        string lastName,
        Email email,
        ContactType type,
        PhoneNumber? phoneNumber,
        string? jobTitle,
        string? department,
        Guid? customerId)
    {
        if (Status == ContactStatus.Archived)
            throw new DomainException("Cannot update archived contact");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Type = type;
        PhoneNumber = phoneNumber;
        JobTitle = jobTitle;
        Department = department;

        if (CustomerId != customerId)
        {
            var oldCustomerId = CustomerId;
            CustomerId = customerId;
            // Publikuj event tylko jeśli była zmiana (z/na Customer)
            if (oldCustomerId.HasValue || customerId.HasValue)
            {
                AddDomainEvent(new ContactAssignedToCustomerEvent(
                    Id,
                    oldCustomerId,
                    customerId, // Teraz może być null
                    DateTime.UtcNow
                ));
            }
        }

        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ContactUpdatedEvent(
            Id,
            "Contact updated",
            DateTime.UtcNow
        ));
    }
}
