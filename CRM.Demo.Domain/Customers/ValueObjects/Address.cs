using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Customers.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    private Address(string street, string city, string postalCode, string country)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    public static Address Create(string street, string city, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street cannot be empty");

        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City cannot be empty");

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Postal code cannot be empty");

        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country cannot be empty");

        return new Address(street, city, postalCode, country);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }

    public override string ToString() => $"{Street}, {PostalCode} {City}, {Country}";
}
