using FluentValidation;

namespace Melodia.Application.Common;

internal static class ValidationExtensions
{
    public const int MaxPageSize = 100;

    public static IRuleBuilderOptions<T, int> ValidPage<T>(this IRuleBuilder<T, int> rule) =>
        rule.GreaterThanOrEqualTo(1).WithMessage("Sayfa numarası en az 1 olmalıdır.");

    public static IRuleBuilderOptions<T, int> ValidPageSize<T>(this IRuleBuilder<T, int> rule) =>
        rule.InclusiveBetween(1, MaxPageSize).WithMessage($"Sayfa boyutu 1 ile {MaxPageSize} arasında olmalıdır.");
}