using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Contacts.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;
}
