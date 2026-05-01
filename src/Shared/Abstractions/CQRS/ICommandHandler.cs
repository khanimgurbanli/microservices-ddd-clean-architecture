using CSharpFunctionalExtensions;
using MediatR;
using Shared.Errors;

namespace Shared.Abstractions.CQRS;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
