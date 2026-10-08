using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Albums.CreateAlbum;

public sealed record CreateAlbumCommand(string Title, Guid ArtistId, DateOnly ReleaseDate) : ICommand<Guid>;