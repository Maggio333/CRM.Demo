namespace CRM.Demo.Domain.Common;

/// <summary>
/// Wyjątek domenowy - używany dla błędów biznesowych w Domain layer.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
    
    public DomainException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
