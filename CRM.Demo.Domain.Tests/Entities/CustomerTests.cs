using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Customers.DomainEvents;
using CRM.Demo.Domain.Customers.Entities;
using CRM.Demo.Domain.Customers.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CRM.Demo.Domain.Tests.Entities;

public class CustomerTests
{
    [Fact]
    public void Create_ValidData_ShouldCreateCustomer()
    {
        // Arrange
        var companyName = "Test Company";
        var taxId = "1234567890";
        var email = Email.Create("test@example.com");

        // Act
        var customer = Customer.Create(companyName, taxId, email);

        // Assert
        customer.Should().NotBeNull();
        customer.Id.Should().NotBeEmpty();
        customer.CompanyName.Should().Be(companyName);
        customer.TaxId.Should().Be(taxId);
        customer.Email.Should().Be(email);
        customer.Status.Should().Be(CustomerStatus.Prospect);
        customer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_ValidData_ShouldPublishCustomerCreatedEvent()
    {
        // Arrange
        var companyName = "Test Company";
        var taxId = "1234567890";
        var email = Email.Create("test@example.com");

        // Act
        var customer = Customer.Create(companyName, taxId, email);

        // Assert
        customer.DomainEvents.Should().HaveCount(1);
        customer.DomainEvents.First().Should().BeOfType<CustomerCreatedEvent>();

        var domainEvent = customer.DomainEvents.First() as CustomerCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.CustomerId.Should().Be(customer.Id);
        domainEvent.CompanyName.Should().Be(companyName);
        domainEvent.Email.Should().Be(email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_EmptyCompanyName_ShouldThrowDomainException(string? companyName)
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        var act = () => Customer.Create(companyName!, "1234567890", email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Company name cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_EmptyTaxId_ShouldThrowDomainException(string? taxId)
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        var act = () => Customer.Create("Test Company", taxId!, email);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Tax ID cannot be empty");
    }

    [Fact]
    public void Create_WithPhoneNumber_ShouldSetPhoneNumber()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var phoneNumber = PhoneNumber.Create("48", "123456789");

        // Act
        var customer = Customer.Create("Test Company", "1234567890", email, phoneNumber);

        // Assert
        customer.PhoneNumber.Should().Be(phoneNumber);
    }

    [Fact]
    public void UpdateContactInfo_ValidData_ShouldUpdateContactInfo()
    {
        // Arrange
        var customer = Customer.Create(
            "Test Company",
            "1234567890",
            Email.Create("old@example.com")
        );
        var newEmail = Email.Create("new@example.com");
        var newPhone = PhoneNumber.Create("48", "987654321");
        var newAddress = Address.Create("New Street", "New City", "12345", "Poland");

        // Act
        customer.UpdateContactInfo(newEmail, newPhone, newAddress);

        // Assert
        customer.Email.Should().Be(newEmail);
        customer.PhoneNumber.Should().Be(newPhone);
        customer.Address.Should().Be(newAddress);
        customer.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateContactInfo_EmailChanged_ShouldPublishCustomerUpdatedEvent()
    {
        // Arrange
        var customer = Customer.Create(
            "Test Company",
            "1234567890",
            Email.Create("old@example.com")
        );
        customer.ClearDomainEvents(); // Clear creation event

        var newEmail = Email.Create("new@example.com");

        // Act
        customer.UpdateContactInfo(newEmail, null, null);

        // Assert
        customer.DomainEvents.Should().HaveCount(1);
        customer.DomainEvents.First().Should().BeOfType<CustomerUpdatedEvent>();
    }

    [Fact]
    public void UpdateContactInfo_EmailNotChanged_ShouldNotPublishEvent()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var customer = Customer.Create("Test Company", "1234567890", email);
        customer.ClearDomainEvents(); // Clear creation event

        // Act
        customer.UpdateContactInfo(email, null, null);

        // Assert
        customer.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void UpdateContactInfo_ArchivedCustomer_ShouldThrowDomainException()
    {
        // Arrange
        var customer = Customer.Create(
            "Test Company",
            "1234567890",
            Email.Create("test@example.com")
        );
        customer.ChangeStatus(CustomerStatus.Archived);
        var newEmail = Email.Create("new@example.com");

        // Act
        var act = () => customer.UpdateContactInfo(newEmail, null, null);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot update archived customer");
    }

    [Fact]
    public void ChangeStatus_NewStatus_ShouldUpdateStatus()
    {
        // Arrange
        var customer = Customer.Create(
            "Test Company",
            "1234567890",
            Email.Create("test@example.com")
        );
        customer.ClearDomainEvents(); // Clear creation event

        // Act
        customer.ChangeStatus(CustomerStatus.Active);

        // Assert
        customer.Status.Should().Be(CustomerStatus.Active);
        customer.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ChangeStatus_NewStatus_ShouldPublishCustomerStatusChangedEvent()
    {
        // Arrange
        var customer = Customer.Create(
            "Test Company",
            "1234567890",
            Email.Create("test@example.com")
        );
        customer.ClearDomainEvents(); // Clear creation event

        // Act
        customer.ChangeStatus(CustomerStatus.Active);

        // Assert
        customer.DomainEvents.Should().HaveCount(1);
        customer.DomainEvents.First().Should().BeOfType<CustomerStatusChangedEvent>();

        var domainEvent = customer.DomainEvents.First() as CustomerStatusChangedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.CustomerId.Should().Be(customer.Id);
        domainEvent.OldStatus.Should().Be(CustomerStatus.Prospect.Value);
        domainEvent.NewStatus.Should().Be(CustomerStatus.Active.Value);
    }

    [Fact]
    public void ChangeStatus_SameStatus_ShouldNotPublishEvent()
    {
        // Arrange
        var customer = Customer.Create(
            "Test Company",
            "1234567890",
            Email.Create("test@example.com")
        );
        customer.ClearDomainEvents(); // Clear creation event

        // Act
        customer.ChangeStatus(CustomerStatus.Prospect); // Same status

        // Assert
        customer.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AssignSalesRep_NewSalesRep_ShouldAssignSalesRep()
    {
        // Arrange
        var customer = Customer.Create(
            "Test Company",
            "1234567890",
            Email.Create("test@example.com")
        );
        var salesRepId = Guid.NewGuid();

        // Act
        customer.AssignSalesRep(salesRepId);

        // Assert
        customer.AssignedSalesRepId.Should().Be(salesRepId);
        customer.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void AssignSalesRep_SameSalesRep_ShouldNotUpdate()
    {
        // Arrange
        var customer = Customer.Create(
            "Test Company",
            "1234567890",
            Email.Create("test@example.com")
        );
        var salesRepId = Guid.NewGuid();
        customer.AssignSalesRep(salesRepId);
        var originalUpdatedAt = customer.UpdatedAt;

        // Wait a bit to ensure time difference
        Thread.Sleep(100);

        // Act
        customer.AssignSalesRep(salesRepId);

        // Assert
        customer.AssignedSalesRepId.Should().Be(salesRepId);
        // UpdatedAt should not change if same sales rep (but it might be null initially)
        if (customer.UpdatedAt.HasValue && originalUpdatedAt.HasValue)
        {
            customer.UpdatedAt.Should().BeCloseTo(originalUpdatedAt.Value, TimeSpan.FromMilliseconds(50));
        }
    }

    [Fact]
    public void ClearDomainEvents_ShouldClearAllEvents()
    {
        // Arrange
        var customer = Customer.Create(
            "Test Company",
            "1234567890",
            Email.Create("test@example.com")
        );
        customer.ChangeStatus(CustomerStatus.Active);

        // Act
        customer.ClearDomainEvents();

        // Assert
        customer.DomainEvents.Should().BeEmpty();
    }
}
