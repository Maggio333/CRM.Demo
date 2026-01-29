using CRM.Demo.Application.Customers.Commands.CreateCustomer;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace CRM.Demo.Application.Tests.Validators;

public class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator;

    public CreateCustomerCommandValidatorTests()
    {
        _validator = new CreateCustomerCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            CompanyName = "Test Company",
            TaxId = "1234567890",
            Email = "test@example.com",
            PhoneNumber = "123456789"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyCompanyName_ShouldFail(string? companyName)
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            CompanyName = companyName!,
            TaxId = "1234567890",
            Email = "test@example.com"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CompanyName" && e.ErrorMessage == "Company name is required");
    }

    [Fact]
    public void Validate_CompanyNameExceeds200Characters_ShouldFail()
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            CompanyName = new string('A', 201), // 201 characters
            TaxId = "1234567890",
            Email = "test@example.com"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CompanyName" && e.ErrorMessage == "Company name must not exceed 200 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyTaxId_ShouldFail(string? taxId)
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            CompanyName = "Test Company",
            TaxId = taxId!,
            Email = "test@example.com"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TaxId" && e.ErrorMessage == "Tax ID is required");
    }

    [Theory]
    [InlineData("123456789")]    // 9 characters
    [InlineData("12345678901")]  // 11 characters
    public void Validate_TaxIdNot10Characters_ShouldFail(string taxId)
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            CompanyName = "Test Company",
            TaxId = taxId,
            Email = "test@example.com"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TaxId" && e.ErrorMessage == "Tax ID must be 10 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyEmail_ShouldFail(string? email)
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            CompanyName = "Test Company",
            TaxId = "1234567890",
            Email = email!
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email is required");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void Validate_InvalidEmailFormat_ShouldFail(string email)
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            CompanyName = "Test Company",
            TaxId = "1234567890",
            Email = email
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Invalid email format");
    }

    [Theory]
    [InlineData("123456789")]        // 9 digits - valid
    [InlineData("+48123456789")]    // +48 prefix - valid
    [InlineData("48123456789")]     // 48 prefix - valid
    [InlineData("+48 123 456 789")] // With spaces - valid
    public void Validate_ValidPhoneNumber_ShouldPass(string phoneNumber)
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            CompanyName = "Test Company",
            TaxId = "1234567890",
            Email = "test@example.com",
            PhoneNumber = phoneNumber
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("12345678")]    // 8 digits
    [InlineData("1234567890")]  // 10 digits
    [InlineData("12345")]       // 5 digits
    [InlineData("12345678a")]   // Contains letter
    public void Validate_InvalidPhoneNumber_ShouldFail(string phoneNumber)
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            CompanyName = "Test Company",
            TaxId = "1234567890",
            Email = "test@example.com",
            PhoneNumber = phoneNumber
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PhoneNumber" && e.ErrorMessage == "Phone number must be 9 digits (with or without country code +48)");
    }

    [Fact]
    public void Validate_EmptyPhoneNumber_ShouldPass()
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            CompanyName = "Test Company",
            TaxId = "1234567890",
            Email = "test@example.com",
            PhoneNumber = null // Optional field
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmailExceeds255Characters_ShouldFail()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com"; // > 255 characters
        var command = new CreateCustomerCommand
        {
            CompanyName = "Test Company",
            TaxId = "1234567890",
            Email = longEmail
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email must not exceed 255 characters");
    }
}
