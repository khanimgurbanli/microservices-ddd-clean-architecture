using MediatR;

namespace Shared.Abstractions.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}