using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Songs.AssignSongToAlbum;

public sealed record AssignSongToAlbumCommand(Guid SongId, Guid AlbumId) : ICommand;