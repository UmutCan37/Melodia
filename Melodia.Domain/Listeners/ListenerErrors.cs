using Melodia.Domain.Abstractions;

namespace Melodia.Domain.Listeners;

public static class ListenerErrors
{
    public static Error NotFound(Guid listenerId) =>
        Error.NotFound("Listener.NotFound", $"'{listenerId}' Id'li dinleyici bulunamadı.");

    public static readonly Error DisplayNameEmpty =
        Error.Validation("Listener.DisplayNameEmpty", "Görünen ad boş olamaz.");

    public static Error DisplayNameTooLong(int maxLength) =>
        Error.Validation("Listener.DisplayNameTooLong", $"Görünen ad en fazla {maxLength} karakter olabilir.");

    public static readonly Error SamePackage =
        Error.Conflict("Listener.SamePackage", "Zaten bu paketi kullanıyorsunuz.");

    public static readonly Error EmailAlreadyInUse =
        Error.Conflict("Listener.EmailAlreadyInUse", "Bu e-posta adresi zaten kullanılıyor.");
}