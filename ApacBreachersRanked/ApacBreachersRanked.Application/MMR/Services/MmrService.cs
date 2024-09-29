using ApacBreachersRanked.Application.MMR.Queries;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;

namespace ApacBreachersRanked.Application.MMR.Services
{
    internal class MMRService : IMmrService
    {
        private readonly IMediator _mediator;

        public MMRService(IMediator mediator)
        {
            _mediator = mediator;
        }
        public Task<List<PlayerMMR>> GetPlayerMmRsAsync(IEnumerable<IUser> users, MatchFormat matchFormat, CancellationToken cancellationToken = default)
            => _mediator.Send(new GetPlayerMMRsQuery { Users = users, MatchFormat = matchFormat }, cancellationToken);
    }
}
