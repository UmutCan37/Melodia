using FluentValidation;
using MediatR;
using Melodia.Domain.Abstractions;

namespace Melodia.Application.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Select(f => new Error(f.PropertyName, f.ErrorMessage, ErrorType.Validation))
            .Distinct()
            .ToArray();

        if (errors.Length == 0)
            return await next(cancellationToken);

        return CreateValidationResult(new ValidationError(errors));
    }

    private static TResponse CreateValidationResult(ValidationError error)
    {
        // Değer döndürmeyen command: Result
        if (typeof(TResponse) == typeof(Result))
            return (Result.Failure(error) as TResponse)!;

        // Değer döndüren command/query: Result<T>
        var valueType = typeof(TResponse).GetGenericArguments()[0];

        var failure = typeof(Result)
            .GetMethods()
            .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethod)
            .MakeGenericMethod(valueType)
            .Invoke(null, [error]);

        return (TResponse)failure!;
    }
}