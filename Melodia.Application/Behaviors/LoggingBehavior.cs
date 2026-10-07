using System.Diagnostics;
using MediatR;
using Melodia.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace Melodia.Application.Behaviors;

internal sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var startTime = Stopwatch.GetTimestamp();

        logger.LogInformation("İstek başladı: {RequestName}", requestName);

        var result = await next(cancellationToken);

        var elapsedMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;

        if (result.IsFailure)
        {
            logger.LogWarning(
                "İstek başarısız: {RequestName} | Hata: {ErrorCode} | {ElapsedMs} ms",
                requestName, result.Error.Code, elapsedMs);
        }
        else
        {
            logger.LogInformation(
                "İstek tamamlandı: {RequestName} | {ElapsedMs} ms",
                requestName, elapsedMs);
        }

        return result;
    }
}