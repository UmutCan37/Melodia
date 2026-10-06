using Melodia.Domain.Abstractions;
using Melodia.Domain.Listenings.Events;

namespace Melodia.Domain.Listenings;

public sealed class ListeningRecord : Entity
{
    private ListeningRecord(Guid id, Guid listenerId, Guid songId, DateTime listenedAtUtc) : base(id)
    {
        ListenerId = listenerId;
        SongId = songId;
        ListenedAtUtc = listenedAtUtc;
    }

    // EF Core için
    private ListeningRecord() { }

    public Guid ListenerId { get; private set; }
    public Guid SongId { get; private set; }
    public DateTime ListenedAtUtc { get; private set; }

    public static ListeningRecord Create(Guid listenerId, Guid songId, DateTime listenedAtUtc)
    {
        var record = new ListeningRecord(Guid.CreateVersion7(), listenerId, songId, listenedAtUtc);

        record.RaiseDomainEvent(new SongListenedEvent(record.Id, listenerId, songId));

        return record;
    }
}