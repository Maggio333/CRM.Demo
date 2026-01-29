namespace CRM.Demo.Domain.Common;

/// <summary>
/// Base class dla Value Objects.
/// Value Objects są porównywane po wartościach, nie referencjach.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Zwraca komponenty używane do porównania równości.
    /// </summary>
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;
        
        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }
    
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        
        if (left is null || right is null)
            return false;
        
        return left.Equals(right);
    }
    
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
