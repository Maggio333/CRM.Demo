using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Customers.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }
    
    private Email(string value)
    {
        Value = value;
    }
    
    // Factory method z walidacją
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");
        
        if (!IsValidEmail(email))
            throw new DomainException($"Invalid email format: {email}");
        
        return new Email(email.ToLowerInvariant().Trim());
    }
    
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    // Value Objects są porównywane po wartościach, nie referencjach
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    // Implicit conversion dla wygody
    public static implicit operator string(Email email) => email.Value;
    
    public override string ToString() => Value;
}
