using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.Moderation.Models;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Moderation.Queries
{
    public class IsUserBannedQuery : IQuery<bool>
    {
        public IUserId UserId { get; set; } = null!;
    }

    public class IsUserBannedHandler : IQueryHandler<IsUserBannedQuery, bool>
    {
        private readonly IMediator _mediator;

        public IsUserBannedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<bool> Handle(IsUserBannedQuery request, CancellationToken cancellationToken)
        {
            IQueryable<UserBan> activeBane = await _mediator.Send(new GetActiveBansQuery(), cancellationToken);
            return await activeBane.AnyAsync(x => x.UserId.Equals(request.UserId), cancellationToken);
        }
    }
}
