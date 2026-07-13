using System.Text.RegularExpressions;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Domain.ValueObjects;

public class Email
{
    public string Value { get; private set; }

    private Email() { }

    public Email(string email)
    {
        if (!IsValid(email))
            throw new ErrorOnValidationException(new List<string> { ResourceMessagesException.INVALID_EMAIL });

        Value = email.Trim().ToLowerInvariant();
    }

    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return regex.IsMatch(email.Trim());
    }

    public override string ToString() => Value;
    public static implicit operator string(Email email) => email.Value;
}