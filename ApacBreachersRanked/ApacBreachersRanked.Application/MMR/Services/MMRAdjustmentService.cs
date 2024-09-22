using ApacBreachersRanked.Application.MMR.Queries;
using ApacBreachersRanked.Domain.Match.Enums;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;

namespace ApacBreachersRanked.Application.MMR.Services
{
    internal class MMRAdjustmentService : IMMRAdjustmentService
    {
        private readonly IMediator _mediator;

        public MMRAdjustmentService(IMediator mediator)
        {
            _mediator = mediator;
        }

        Task<List<PlayerMMR>> IMMRAdjustmentService.GetPlayerMMRsAsync(IEnumerable<IUser> users, MatchFormat matchFormat, CancellationToken cancellationToken)
            => _mediator.Send(new GetPlayerMMRsQuery { Users = users, MatchFormat = matchFormat }, cancellationToken);
    }
}
