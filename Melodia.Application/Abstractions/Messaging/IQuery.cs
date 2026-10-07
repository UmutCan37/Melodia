using MediatR;
using Melodia.Domain.Abstractions;

namespace Melodia.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;