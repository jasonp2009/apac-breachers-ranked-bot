using MediatR;

namespace ApacBreachersRanked.Application.Common.Mediator
{
    public interface IQuery<TResponse> : IRequest<TResponse>
    {

    }
}
