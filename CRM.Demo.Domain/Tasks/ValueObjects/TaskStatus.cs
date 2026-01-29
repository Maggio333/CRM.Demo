using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Tasks.ValueObjects;

public class TaskStatus : ValueObject
{
    public static readonly TaskStatus ToDo = new("ToDo", "Task to be done");
    public static readonly TaskStatus InProgress = new("InProgress", "Task in progress");
    public static readonly TaskStatus Completed = new("Completed", "Task completed");
    public static readonly TaskStatus Cancelled = new("Cancelled", "Task cancelled");
    public static readonly TaskStatus OnHold = new("OnHold", "Task on hold");
    
    public string Value { get; }
    public string Description { get; }
    
    private TaskStatus(string value, string description)
    {
        Value = value;
        Description = description;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static TaskStatus FromString(string value)
    {
        return value switch
        {
            "ToDo" => ToDo,
            "InProgress" => InProgress,
            "Completed" => Completed,
            "Cancelled" => Cancelled,
            "OnHold" => OnHold,
            _ => throw new DomainException($"Invalid task status: {value}")
        };
    }
    
    public override string ToString() => Value;
}
