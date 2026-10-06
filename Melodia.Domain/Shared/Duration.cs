using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Shared;

public sealed record Duration
{
    private Duration(int totalSeconds)
    {
        TotalSeconds = totalSeconds;
    }

    public int TotalSeconds { get; }

    public static Result<Duration> FromSeconds(int totalSeconds)
    {
        if (totalSeconds <= 0)
            return Error.Validation("Duration.Invalid", "Süre sıfırdan büyük olmalıdır.");

        return new Duration(totalSeconds);
    }

    // 225 → "3:45"
    public override string ToString() => $"{TotalSeconds / 60}:{TotalSeconds % 60:D2}";
}