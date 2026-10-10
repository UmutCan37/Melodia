using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Songs.GetSongById;

public sealed record GetSongByIdQuery(Guid SongId) : IQuery<SongResponse>;