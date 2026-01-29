using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Customers.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CRM.Demo.Domain.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Fact]
    public void Create_ValidPhoneNumber_ShouldCreatePhoneNumber()
    {
        // Arrange
        var countryCode = "48";
        var number = "123456789";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        result.Should().NotBeNull();
        result.CountryCode.Should().Be(countryCode);
        result.Number.Should().Be(number);
        result.FullNumber.Should().Be("+48123456789");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_EmptyCountryCode_ShouldThrowDomainException(string? countryCode)
    {
        // Act
        var act = () => PhoneNumber.Create(countryCode!, "123456789");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Country code cannot be empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_EmptyNumber_ShouldThrowDomainException(string? number)
    {
        // Act
        var act = () => PhoneNumber.Create("48", number!);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Phone number cannot be empty");
    }

    [Theory]
    [InlineData("12345678")]    // 8 digits
    [InlineData("1234567890")]  // 10 digits
    [InlineData("12345")]       // 5 digits
    [InlineData("12345678a")]   // Contains letter
    [InlineData("123-456-789")] // Contains dashes
    [InlineData("123 456 789")] // Contains spaces
    public void Create_InvalidNumberFormat_ShouldThrowDomainException(string number)
    {
        // Act
        var act = () => PhoneNumber.Create("48", number);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Phone number must be 9 digits");
    }

    [Fact]
    public void Create_Valid9Digits_ShouldSucceed()
    {
        // Arrange
        var validNumbers = new[] { "123456789", "987654321", "555123456" };

        foreach (var number in validNumbers)
        {
            // Act
            var result = PhoneNumber.Create("48", number);

            // Assert
            result.Number.Should().Be(number);
        }
    }

    [Fact]
    public void FullNumber_ShouldFormatCorrectly()
    {
        // Arrange
        var phoneNumber = PhoneNumber.Create("48", "123456789");

        // Act
        var fullNumber = phoneNumber.FullNumber;

        // Assert
        fullNumber.Should().Be("+48123456789");
    }

    [Fact]
    public void Equals_SameValues_ShouldReturnTrue()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("48", "123456789");
        var phone2 = PhoneNumber.Create("48", "123456789");

        // Act & Assert
        phone1.Should().Be(phone2);
        phone1.GetHashCode().Should().Be(phone2.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentCountryCode_ShouldReturnFalse()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("48", "123456789");
        var phone2 = PhoneNumber.Create("49", "123456789");

        // Act & Assert
        phone1.Should().NotBe(phone2);
    }

    [Fact]
    public void Equals_DifferentNumber_ShouldReturnFalse()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("48", "123456789");
        var phone2 = PhoneNumber.Create("48", "987654321");

        // Act & Assert
        phone1.Should().NotBe(phone2);
    }

    [Fact]
    public void ToString_ShouldReturnFullNumber()
    {
        // Arrange
        var phoneNumber = PhoneNumber.Create("48", "123456789");

        // Act
        var result = phoneNumber.ToString();

        // Assert
        result.Should().Be("+48123456789");
    }
}
