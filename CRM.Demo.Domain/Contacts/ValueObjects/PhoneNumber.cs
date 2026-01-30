using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Contacts.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string CountryCode { get; }
    public string Number { get; }
    public string FullNumber => $"+{CountryCode}{Number}";

    private PhoneNumber(string countryCode, string number)
    {
        CountryCode = countryCode;
        Number = number;
    }

    public static PhoneNumber Create(string countryCode, string number)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new DomainException("Country code cannot be empty");

        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("Phone number cannot be empty");

        if (!System.Text.RegularExpressions.Regex.IsMatch(number, @"^\d{9}$"))
            throw new DomainException("Phone number must be 9 digits");

        return new PhoneNumber(countryCode, number);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return Number;
    }

    public override string ToString() => FullNumber;
}
