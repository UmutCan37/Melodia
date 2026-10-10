using Melodia.Application.Abstractions.Messaging;

namespace Melodia.Application.Features.Songs.StreamSong;

public sealed record StreamSongQuery(Guid SongId, long Expires, string Signature) : IQuery<Stream>;