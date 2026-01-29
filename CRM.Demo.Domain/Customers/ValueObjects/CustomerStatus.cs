using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Customers.ValueObjects;

public class CustomerStatus : ValueObject
{
    public static readonly CustomerStatus Prospect = new("Prospect", "New potential customer");
    public static readonly CustomerStatus Active = new("Active", "Active customer");
    public static readonly CustomerStatus Inactive = new("Inactive", "Inactive customer");
    public static readonly CustomerStatus Archived = new("Archived", "Archived customer");
    
    public string Value { get; }
    public string Description { get; }
    
    private CustomerStatus(string value, string description)
    {
        Value = value;
        Description = description;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static CustomerStatus FromString(string value)
    {
        return value switch
        {
            "Prospect" => Prospect,
            "Active" => Active,
            "Inactive" => Inactive,
            "Archived" => Archived,
            _ => throw new DomainException($"Invalid customer status: {value}")
        };
    }
    
    public override string ToString() => Value;
}
