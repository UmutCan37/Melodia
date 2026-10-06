using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Listenings.Events;

public sealed record SongListenedEvent(
    Guid ListeningRecordId,
    Guid ListenerId,
    Guid SongId) : IDomainEvent;