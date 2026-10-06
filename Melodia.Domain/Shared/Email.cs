using System.Text.RegularExpressions;
using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Shared;

public sealed partial record Email
{
    private const int MaxLength = 256;

    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Error.Validation("Email.Empty", "E-posta boş olamaz.");

        email = email.Trim().ToLowerInvariant();

        if (email.Length > MaxLength)
            return Error.Validation("Email.TooLong", $"E-posta en fazla {MaxLength} karakter olabilir.");

        if (!EmailRegex().IsMatch(email))
            return Error.Validation("Email.InvalidFormat", "E-posta formatı geçersiz.");

        return new Email(email);
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}