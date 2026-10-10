using Melodia.Presentation.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace Melodia.Presentation;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapPresentationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapMeEndpoints();
        app.MapGenreEndpoints();
        app.MapArtistEndpoints();
        app.MapAlbumEndpoints();
        app.MapSongEndpoints();
        app.MapStreamEndpoints();
        app.MapPlaylistEndpoints();

        return app;
    }
}