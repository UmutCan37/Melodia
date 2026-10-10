using MediatR;
using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Features.Songs.AssignSongToAlbum;
using Melodia.Application.Features.Songs.CreateSong;
using Melodia.Application.Features.Songs.DeleteSong;
using Melodia.Application.Features.Songs.GetSongById;
using Melodia.Application.Features.Songs.GetSongs;
using Melodia.Application.Features.Songs.PlaySong;
using Melodia.Application.Features.Songs.RemoveSongFromAlbum;
using Melodia.Application.Features.Songs.UpdateSong;
using Melodia.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Melodia.Presentation.Endpoints;

public static class SongEndpoints
{
    private const long MaxAudioFileSize = 20 * 1024 * 1024; // 20 MB

    public sealed record CreateSongForm(
        string Title,
        int DurationSeconds,
        Guid ArtistId,
        Guid GenreId,
        Guid? AlbumId,
        string RequiredPackage,
        IFormFile Audio);

    public sealed record UpdateSongRequest(string Title, string RequiredPackage);
    public sealed record AssignToAlbumRequest(Guid AlbumId);

    public static IEndpointRouteBuilder MapSongEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/songs").WithTags("Songs");

        group.MapGet("/", async ([AsParameters] GetSongsQuery query, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        })
        .AllowAnonymous();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSongByIdQuery(id), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        })
        .AllowAnonymous();

        // Projenin kalbi: giriş yapmış her kullanıcı, paketi yetiyorsa
        group.MapPost("/{id:guid}/play", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new PlaySongCommand(id), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        })
        .RequireAuthorization();

        var admin = group.MapGroup("").RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        admin.MapPost("/", async ([FromForm] CreateSongForm form, ISender sender, CancellationToken ct) =>
        {
            if (form.Audio is null || form.Audio.Length == 0)
                return Results.Problem(statusCode: 400, title: "Bad Request", detail: "Ses dosyası zorunludur.");

            if (form.Audio.Length > MaxAudioFileSize)
                return Results.Problem(statusCode: 413, title: "Payload Too Large", detail: "Ses dosyası en fazla 20 MB olabilir.");

            await using var audio = form.Audio.OpenReadStream();

            var command = new CreateSongCommand(
                form.Title,
                form.DurationSeconds,
                form.ArtistId,
                form.GenreId,
                form.AlbumId,
                form.RequiredPackage,
                audio,
                form.Audio.FileName);

            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/songs/{result.Value}", new { id = result.Value })
                : result.ToProblem();
        })
        .DisableAntiforgery()
        .WithMetadata(new RequestSizeLimitAttribute(MaxAudioFileSize + 1024 * 1024));

        admin.MapPut("/{id:guid}", async (Guid id, UpdateSongRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateSongCommand(id, request.Title, request.RequiredPackage), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        admin.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteSongCommand(id), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        admin.MapPut("/{id:guid}/album", async (Guid id, AssignToAlbumRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new AssignSongToAlbumCommand(id, request.AlbumId), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        admin.MapDelete("/{id:guid}/album", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RemoveSongFromAlbumCommand(id), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        return app;
    }
}