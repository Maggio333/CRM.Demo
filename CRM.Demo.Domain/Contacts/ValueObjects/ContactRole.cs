using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Contacts.ValueObjects;

public class ContactRole : ValueObject
{
    public static readonly ContactRole DecisionMaker = new("DecisionMaker", "Makes final decisions");
    public static readonly ContactRole Influencer = new("Influencer", "Influences decisions");
    public static readonly ContactRole User = new("User", "End user of product/service");
    public static readonly ContactRole Technical = new("Technical", "Technical evaluator");
    public static readonly ContactRole Financial = new("Financial", "Financial evaluator");
    
    public string Value { get; }
    public string Description { get; }
    
    private ContactRole(string value, string description)
    {
        Value = value;
        Description = description;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static ContactRole FromString(string value)
    {
        return value switch
        {
            "DecisionMaker" => DecisionMaker,
            "Influencer" => Influencer,
            "User" => User,
            "Technical" => Technical,
            "Financial" => Financial,
            _ => throw new DomainException($"Invalid contact role: {value}")
        };
    }
    
    public override string ToString() => Value;
}
