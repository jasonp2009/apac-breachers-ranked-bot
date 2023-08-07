using ApacBreachersRanked.Application.MMR.Queries;
using ApacBreachersRanked.Domain.Match.Services;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;

namespace ApacBreachersRanked.Application.Match.Services
{
    internal class MatchService : IMatchService
    {
        private readonly IMediator _mediator;
        public MatchService(IMediator mediator)
        {
            _mediator = mediator;
        }
        Task<List<PlayerMMR>> IMatchService.GetPlayerMMRsAsync(IEnumerable<IUser> users, CancellationToken cancellationToken)
            => _mediator.Send(new GetPlayerMMRsQuery { Users = users }, cancellationToken);
    }
}
