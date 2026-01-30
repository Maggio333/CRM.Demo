using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Notes.ValueObjects;

public class NoteCategory : ValueObject
{
    public static readonly NoteCategory Sales = new("Sales", "Sales-related note");
    public static readonly NoteCategory Support = new("Support", "Support-related note");
    public static readonly NoteCategory Marketing = new("Marketing", "Marketing-related note");
    public static readonly NoteCategory General = new("General", "General category");
    public static readonly NoteCategory Legal = new("Legal", "Legal-related note");

    public string Value { get; }
    public string Description { get; }

    private NoteCategory(string value, string description)
    {
        Value = value;
        Description = description;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static NoteCategory FromString(string value)
    {
        return value switch
        {
            "Sales" => Sales,
            "Support" => Support,
            "Marketing" => Marketing,
            "General" => General,
            "Legal" => Legal,
            _ => throw new DomainException($"Invalid note category: {value}")
        };
    }

    public override string ToString() => Value;
}
