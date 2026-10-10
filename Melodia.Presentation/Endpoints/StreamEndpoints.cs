using MediatR;
using Melodia.Application.Features.Songs.StreamSong;
using Melodia.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Melodia.Presentation.Endpoints;

public static class StreamEndpoints
{
    public static IEndpointRouteBuilder MapStreamEndpoints(this IEndpointRouteBuilder app)
    {
        
        app.MapGet("/api/stream/{id:guid}", async (
                Guid id,
                long expires,
                string signature,
                ISender sender,
                CancellationToken ct) =>
        {
            var result = await sender.Send(new StreamSongQuery(id, expires, signature), ct);

            return result.IsSuccess
                ? Results.File(result.Value, "audio/mpeg", enableRangeProcessing: true)
                : result.ToProblem();
        })
            .AllowAnonymous()
            .WithTags("Stream");

        return app;
    }
}