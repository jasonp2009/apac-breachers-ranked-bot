﻿using MediatR;

namespace ApacBreachersRanked.Application.Common.Mediator
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Unit>
        where TCommand : ICommand
    {

    }
    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {

    }
}
