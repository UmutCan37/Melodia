using MediatR;
using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Features.Artists.CreateArtist;
using Melodia.Application.Features.Artists.DeleteArtist;
using Melodia.Application.Features.Artists.GetArtistById;
using Melodia.Application.Features.Artists.GetArtists;
using Melodia.Application.Features.Artists.UpdateArtist;
using Melodia.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Melodia.Presentation.Endpoints;

public static class ArtistEndpoints
{
    public sealed record UpdateArtistRequest(string Name, string? Biography);

    public static IEndpointRouteBuilder MapArtistEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/artists").WithTags("Artists");

        group.MapGet("/", async ([AsParameters] GetArtistsQuery query, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        })
        .AllowAnonymous();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetArtistByIdQuery(id), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        })
        .AllowAnonymous();

        var admin = group.MapGroup("").RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        admin.MapPost("/", async (CreateArtistCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/artists/{result.Value}", new { id = result.Value })
                : result.ToProblem();
        });

        admin.MapPut("/{id:guid}", async (Guid id, UpdateArtistRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateArtistCommand(id, request.Name, request.Biography), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        admin.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteArtistCommand(id), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        return app;
    }
}