using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Contacts.ValueObjects;

public class ContactStatus : ValueObject
{
    public static readonly ContactStatus Active = new("Active", "Active contact");
    public static readonly ContactStatus Inactive = new("Inactive", "Inactive contact");
    public static readonly ContactStatus Archived = new("Archived", "Archived contact");

    public string Value { get; }
    public string Description { get; }

    private ContactStatus(string value, string description)
    {
        Value = value;
        Description = description;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static ContactStatus FromString(string value)
    {
        return value switch
        {
            "Active" => Active,
            "Inactive" => Inactive,
            "Archived" => Archived,
            _ => throw new DomainException($"Invalid contact status: {value}")
        };
    }

    public override string ToString() => Value;
}
