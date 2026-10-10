using MediatR;
using Melodia.Application.Features.Playlists.AddSongToPlaylist;
using Melodia.Application.Features.Playlists.CreatePlaylist;
using Melodia.Application.Features.Playlists.DeletePlaylist;
using Melodia.Application.Features.Playlists.GetPlaylistById;
using Melodia.Application.Features.Playlists.RemoveSongFromPlaylist;
using Melodia.Application.Features.Playlists.RenamePlaylist;
using Melodia.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Melodia.Presentation.Endpoints;

public static class PlaylistEndpoints
{
    public sealed record RenamePlaylistRequest(string Name);
    public sealed record AddSongRequest(Guid SongId);

    public static IEndpointRouteBuilder MapPlaylistEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/playlists")
            .WithTags("Playlists")
            .RequireAuthorization();

        group.MapPost("/", async (CreatePlaylistCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/playlists/{result.Value}", new { id = result.Value })
                : result.ToProblem();
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPlaylistByIdQuery(id), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        });

        group.MapPut("/{id:guid}", async (Guid id, RenamePlaylistRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RenamePlaylistCommand(id, request.Name), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeletePlaylistCommand(id), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        group.MapPost("/{id:guid}/songs", async (Guid id, AddSongRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new AddSongToPlaylistCommand(id, request.SongId), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        group.MapDelete("/{id:guid}/songs/{songId:guid}", async (Guid id, Guid songId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RemoveSongFromPlaylistCommand(id, songId), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        return app;
    }
}