using MediatR;
using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Features.Genres.CreateGenre;
using Melodia.Application.Features.Genres.DeleteGenre;
using Melodia.Application.Features.Genres.GetGenres;
using Melodia.Application.Features.Genres.RenameGenre;
using Melodia.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Melodia.Presentation.Endpoints;

public static class GenreEndpoints
{
    public sealed record RenameGenreRequest(string Name);

    public static IEndpointRouteBuilder MapGenreEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/genres").WithTags("Genres");

        
        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetGenresQuery(), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        })
        .AllowAnonymous();

        // Admin işlemleri
        var admin = group.MapGroup("").RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        admin.MapPost("/", async (CreateGenreCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/genres/{result.Value}", new { id = result.Value })
                : result.ToProblem();
        });

        admin.MapPut("/{id:guid}", async (Guid id, RenameGenreRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RenameGenreCommand(id, request.Name), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        admin.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteGenreCommand(id), ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        return app;
    }
}