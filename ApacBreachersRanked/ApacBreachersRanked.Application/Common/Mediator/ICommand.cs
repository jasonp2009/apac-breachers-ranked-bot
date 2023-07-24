using MediatR;

namespace ApacBreachersRanked.Application.Common.Mediator
{
    public interface ICommand : ICommand<Unit>
    {

    }

    public interface ICommand<TReturn> : IRequest<TReturn>
    {

    }
}
