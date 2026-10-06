using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Listeners.Events;

public sealed record ListenerRegisteredEvent(Guid ListenerId) : IDomainEvent;