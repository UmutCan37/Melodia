using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melodia.Domain.Abstractions
{
    // Tüm domain event'lerin taşıdığı etiket.
    // "Bu sınıf, domain'de olmuş önemli bir şeyi anlatır" der.
    public interface IDomainEvent : INotification
    {
    }
}
