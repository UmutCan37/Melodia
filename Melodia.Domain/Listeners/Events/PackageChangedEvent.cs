using Melodia.Domain.Abstractions;
using Melodia.Domain.Shared;

namespace Melodia.Domain.Listeners.Events;

public sealed record PackageChangedEvent(
    Guid ListenerId,
    Package OldPackage,
    Package NewPackage) : IDomainEvent;