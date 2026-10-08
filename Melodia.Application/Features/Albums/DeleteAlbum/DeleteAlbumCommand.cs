using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Albums.DeleteAlbum;

public sealed record DeleteAlbumCommand(Guid AlbumId) : ICommand;