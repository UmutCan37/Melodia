using MediatR;
using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Features.Albums.CreateAlbum;
using Melodia.Application.Features.Albums.DeleteAlbum;
using Melodia.Application.Features.Albums.GetAlbumById;
using Melodia.Application.Features.Albums.GetAlbums;
using Melodia.Application.Features.Albums.UpdateAlbum;
using Melodia.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Melodia.Presentation.Endpoints;

public static class AlbumEndpoints
{
    public sealed record UpdateAlbumRequest(string Title, DateOnly ReleaseDate);

    public static IEndpointRouteBuilder MapAlbumEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/albums").WithTags("Albums");

        group.MapGet("/", async ([AsParameters] GetAlbumsQuery query, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        })
        .AllowAnonymous();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAlbumByIdQuery(id), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        })
        .AllowAnonymous();

        var admin = group.MapGroup("").RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        admin.MapPost("/", async (CreateAlbumCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/albums/{result.Value}", new { id = result.Value })
                : result.ToProblem();
        });

        admin.MapPut("/{id:guid}", async (Guid id, UpdateAlbumRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateAlbumCommand(id, request.Title, request.ReleaseDate), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        admin.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteAlbumCommand(id), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        return app;
    }
}