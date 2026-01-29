using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Contacts.ValueObjects;

public class ContactType : ValueObject
{
    public static readonly ContactType Person = new("Person", "Individual person");
    public static readonly ContactType Company = new("Company", "Company contact");
    
    public string Value { get; }
    public string Description { get; }
    
    private ContactType(string value, string description)
    {
        Value = value;
        Description = description;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static ContactType FromString(string value)
    {
        return value switch
        {
            "Person" => Person,
            "Company" => Company,
            _ => throw new DomainException($"Invalid contact type: {value}")
        };
    }
    
    public override string ToString() => Value;
}
