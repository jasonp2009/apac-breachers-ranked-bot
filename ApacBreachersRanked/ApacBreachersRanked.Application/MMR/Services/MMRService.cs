using ApacBreachersRanked.Application.MMR.Queries;
using ApacBreachersRanked.Domain.MMR.Entities;
using ApacBreachersRanked.Domain.MMR.Services;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;

namespace ApacBreachersRanked.Application.MMR.Services
{
    internal class MMRService : IMMRService
    {
        private readonly IMediator _mediator;

        public MMRService(IMediator mediator)
        {
            _mediator = mediator;
        }
        public Task<List<PlayerMMR>> GetPlayerMMRsAsync(IEnumerable<IUser> users, CancellationToken cancellationToken = default)
            => _mediator.Send(new GetPlayerMMRsQuery { Users = users }, cancellationToken);
    }
}
