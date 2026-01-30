using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Tasks.ValueObjects;

public class TaskType : ValueObject
{
    public static readonly TaskType Call = new("Call", "Phone call");
    public static readonly TaskType Meeting = new("Meeting", "In-person or online meeting");
    public static readonly TaskType Email = new("Email", "Email communication");
    public static readonly TaskType FollowUp = new("FollowUp", "Follow-up action");
    public static readonly TaskType Document = new("Document", "Document preparation or review");
    public static readonly TaskType Other = new("Other", "Other type of task");

    public string Value { get; }
    public string Description { get; }

    private TaskType(string value, string description)
    {
        Value = value;
        Description = description;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static TaskType FromString(string value)
    {
        return value switch
        {
            "Call" => Call,
            "Meeting" => Meeting,
            "Email" => Email,
            "FollowUp" => FollowUp,
            "Document" => Document,
            "Other" => Other,
            _ => throw new DomainException($"Invalid task type: {value}")
        };
    }

    public override string ToString() => Value;
}
