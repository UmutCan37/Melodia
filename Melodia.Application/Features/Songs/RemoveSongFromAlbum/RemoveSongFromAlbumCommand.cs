using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Songs.RemoveSongFromAlbum;

public sealed record RemoveSongFromAlbumCommand(Guid SongId) : ICommand;