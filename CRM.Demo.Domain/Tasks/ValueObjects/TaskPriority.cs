using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Tasks.ValueObjects;

public class TaskPriority : ValueObject
{
    public static readonly TaskPriority Low = new("Low", 1);
    public static readonly TaskPriority Medium = new("Medium", 2);
    public static readonly TaskPriority High = new("High", 3);
    public static readonly TaskPriority Urgent = new("Urgent", 4);
    
    public string Value { get; }
    public int Level { get; }  // Do sortowania
    
    private TaskPriority(string value, int level)
    {
        Value = value;
        Level = level;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static TaskPriority FromString(string value)
    {
        return value switch
        {
            "Low" => Low,
            "Medium" => Medium,
            "High" => High,
            "Urgent" => Urgent,
            _ => throw new DomainException($"Invalid task priority: {value}")
        };
    }
    
    public override string ToString() => Value;
}
