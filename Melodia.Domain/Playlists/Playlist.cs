using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;
using Melodia.Domain.Services;
using Melodia.Domain.Songs;

namespace Melodia.Domain.Playlists;

public sealed class Playlist : Entity
{
    public const int NameMaxLength = 100;

    private readonly List<PlaylistSong> _songs = [];

    private Playlist(Guid id, Guid ownerId, string name) : base(id)
    {
        OwnerId = ownerId;
        Name = name;
    }

    // EF Core için
    private Playlist() { }

    public Guid OwnerId { get; private set; }
    public string Name { get; private set; } = null!;
    public IReadOnlyCollection<PlaylistSong> Songs => _songs.AsReadOnly();

    public static Result<Playlist> Create(Guid ownerId, string name)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        return new Playlist(Guid.CreateVersion7(), ownerId, nameResult.Value);
    }

    public Result EnsureOwner(Guid requesterId) =>
    requesterId == OwnerId ? Result.Success() : PlaylistErrors.NotOwner;

    public Result Rename(Guid requesterId, string name)
    {
        if (requesterId != OwnerId)
            return PlaylistErrors.NotOwner;

        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        Name = nameResult.Value;
        return Result.Success();
    }

    public Result AddSong(Listener requester, Song song, DateTime addedAtUtc)
    {
        if (requester.Id != OwnerId)
            return PlaylistErrors.NotOwner;

        // Çalamayacağın şarkıyı listene ekleyemezsin
        var canPlay = PlaybackPolicy.CanPlay(requester, song);
        if (canPlay.IsFailure)
            return canPlay.Error;

        if (_songs.Any(s => s.SongId == song.Id))
            return PlaylistErrors.SongAlreadyInPlaylist;

        _songs.Add(PlaylistSong.Create(Id, song.Id, _songs.Count + 1, addedAtUtc));
        return Result.Success();
    }

    public Result RemoveSong(Guid requesterId, Guid songId)
    {
        if (requesterId != OwnerId)
            return PlaylistErrors.NotOwner;

        var playlistSong = _songs.FirstOrDefault(s => s.SongId == songId);
        if (playlistSong is null)
            return PlaylistErrors.SongNotInPlaylist;

        _songs.Remove(playlistSong);

        // Aradaki boşluğu kapat: 1, 2, 4 değil 1, 2, 3 olsun
        foreach (var song in _songs.Where(s => s.Position > playlistSong.Position))
            song.MoveTo(song.Position - 1);

        return Result.Success();
    }

    private static Result<string> ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return PlaylistErrors.NameEmpty;

        name = name.Trim();

        if (name.Length > NameMaxLength)
            return PlaylistErrors.NameTooLong(NameMaxLength);

        return name;
    }
}