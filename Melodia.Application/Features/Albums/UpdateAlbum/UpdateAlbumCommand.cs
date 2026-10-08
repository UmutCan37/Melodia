using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Albums.UpdateAlbum;

public sealed record UpdateAlbumCommand(Guid AlbumId, string Title, DateOnly ReleaseDate) : ICommand;