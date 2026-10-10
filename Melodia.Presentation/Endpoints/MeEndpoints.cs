using MediatR;
using Melodia.Application.Features.Listeners.ChangePackage;
using Melodia.Application.Features.Listeners.GetMyProfile;
using Melodia.Application.Features.Listenings.GetRecentlyPlayed;
using Melodia.Application.Features.Listenings.GetRecommendations;
using Melodia.Application.Features.Playlists.GetMyPlaylists;
using Melodia.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Melodia.Presentation.Endpoints;

public static class MeEndpoints
{
    public static IEndpointRouteBuilder MapMeEndpoints(this IEndpointRouteBuilder app)
    {
        // Kimin verisi olduğu URL'den değil token'dan gelir
        var group = app.MapGroup("/api/me")
            .WithTags("Me")
            .RequireAuthorization();

        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMyProfileQuery(), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        });

        group.MapPut("/package", async (ChangePackageCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess ? Results.NoContent() : result.ToProblem();
        });

        group.MapGet("/playlists", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMyPlaylistsQuery(), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        });

        group.MapGet("/recently-played", async ([AsParameters] GetRecentlyPlayedQuery query, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        });

        group.MapGet("/recommendations", async ([AsParameters] GetRecommendationsQuery query, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
        });

        return app;
    }
}