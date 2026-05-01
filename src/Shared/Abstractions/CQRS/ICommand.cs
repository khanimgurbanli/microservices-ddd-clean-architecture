using MediatR;

namespace Shared.Abstractions.CQRS;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand : IRequest
{
}