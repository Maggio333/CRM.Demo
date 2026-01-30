using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Notes.ValueObjects;

public class NoteType : ValueObject
{
    public static readonly NoteType Call = new("Call", "Phone call note");
    public static readonly NoteType Meeting = new("Meeting", "Meeting note");
    public static readonly NoteType Email = new("Email", "Email note");
    public static readonly NoteType Task = new("Task", "Task-related note");
    public static readonly NoteType General = new("General", "General note");
    public static readonly NoteType FollowUp = new("FollowUp", "Follow-up note");
    public static readonly NoteType Document = new("Document", "Document note");

    public string Value { get; }
    public string Description { get; }

    private NoteType(string value, string description)
    {
        Value = value;
        Description = description;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static NoteType FromString(string value)
    {
        return value switch
        {
            "Call" => Call,
            "Meeting" => Meeting,
            "Email" => Email,
            "Task" => Task,
            "General" => General,
            "FollowUp" => FollowUp,
            "Document" => Document,
            _ => throw new DomainException($"Invalid note type: {value}")
        };
    }

    public override string ToString() => Value;
}
