using Melodia.Domain.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Melodia.Presentation.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Başarılı bir sonuç ProblemDetails'e çevrilemez.");

        var error = result.Error;
        var statusCode = GetStatusCode(error.Type);

        return Results.Problem(
            statusCode: statusCode,
            title: GetTitle(statusCode),
            type: GetType(statusCode),
            detail: error.Description,
            extensions: GetExtensions(error));
    }

    private static int GetStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };

    private static string GetTitle(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        _ => "Error"
    };

    private static string GetType(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.5.1"
    };

    private static Dictionary<string, object?> GetExtensions(Error error)
    {
        var extensions = new Dictionary<string, object?> { ["errorCode"] = error.Code };

        // Doğrulama hataları: alan alan, Angular formunun altına yazılabilecek şekilde
        if (error is ValidationError validationError)
        {
            extensions["errors"] = validationError.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
        }

        return extensions;
    }
}