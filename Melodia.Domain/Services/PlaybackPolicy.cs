using Melodia.Domain.Abstractions;
using Melodia.Domain.Listeners;
using Melodia.Domain.Songs;

namespace Melodia.Domain.Services;

public static class PlaybackPolicy
{
    public static Result CanPlay(Listener listener, Song song)
    {
        if (!listener.Package.CanAccess(song.RequiredPackage))
            return SongErrors.PackageNotSufficient;

        return Result.Success();
    }
}