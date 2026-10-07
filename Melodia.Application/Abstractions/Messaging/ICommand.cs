using MediatR;
using Melodia.Domain.Abstractions;

namespace Melodia.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;