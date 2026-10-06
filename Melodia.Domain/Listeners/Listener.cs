using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners.Events;
using Melodia.Domain.Shared;

namespace Melodia.Domain.Listeners;

public sealed class Listener : Entity
{
    public const int DisplayNameMaxLength = 50;

    private Listener(Guid id, Email email, string displayName, Package package, DateTime registeredAtUtc)
        : base(id)
    {
        Email = email;
        DisplayName = displayName;
        Package = package;
        RegisteredAtUtc = registeredAtUtc;
    }

    // EF Core için
    private Listener() { }

    public Email Email { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public Package Package { get; private set; } = null!;
    public DateTime RegisteredAtUtc { get; private set; }

    // Id, Identity'deki kullanıcının Id'si; ikisi aynı kişiyi temsil ediyor
    public static Result<Listener> Register(Guid userId, Email email, string displayName, DateTime registeredAtUtc)
    {
        var displayNameResult = ValidateDisplayName(displayName);
        if (displayNameResult.IsFailure)
            return displayNameResult.Error;

        var listener = new Listener(userId, email, displayNameResult.Value, Package.Basic, registeredAtUtc);

        listener.RaiseDomainEvent(new ListenerRegisteredEvent(listener.Id));

        return listener;
    }

    public Result ChangePackage(Package newPackage)
    {
        if (newPackage == Package)
            return ListenerErrors.SamePackage;

        var oldPackage = Package;
        Package = newPackage;

        RaiseDomainEvent(new PackageChangedEvent(Id, oldPackage, newPackage));

        return Result.Success();
    }

    public Result ChangeDisplayName(string displayName)
    {
        var displayNameResult = ValidateDisplayName(displayName);
        if (displayNameResult.IsFailure)
            return displayNameResult.Error;

        DisplayName = displayNameResult.Value;
        return Result.Success();
    }

    private static Result<string> ValidateDisplayName(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return ListenerErrors.DisplayNameEmpty;

        displayName = displayName.Trim();

        if (displayName.Length > DisplayNameMaxLength)
            return ListenerErrors.DisplayNameTooLong(DisplayNameMaxLength);

        return displayName;
    }
}