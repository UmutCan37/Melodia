namespace Melodia.Infrastructure.Notifications;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string Host { get; init; } = null!;
    public int Port { get; init; } = 587;
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string FromAddress { get; init; } = null!;
    public string FromName { get; init; } = "Melodia";
}