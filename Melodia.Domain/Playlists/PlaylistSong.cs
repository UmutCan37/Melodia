using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Playlists;

public sealed class PlaylistSong : Entity
{
    private PlaylistSong(Guid id, Guid playlistId, Guid songId, int position, DateTime addedAtUtc)
        : base(id)
    {
        PlaylistId = playlistId;
        SongId = songId;
        Position = position;
        AddedAtUtc = addedAtUtc;
    }

    // EF Core için
    private PlaylistSong() { }

    public Guid PlaylistId { get; private set; }
    public Guid SongId { get; private set; }
    public int Position { get; private set; }
    public DateTime AddedAtUtc { get; private set; }

    internal static PlaylistSong Create(Guid playlistId, Guid songId, int position, DateTime addedAtUtc) =>
        new(Guid.CreateVersion7(), playlistId, songId, position, addedAtUtc);

    internal void MoveTo(int position) => Position = position;
}