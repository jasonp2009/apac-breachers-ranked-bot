using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.Moderation.Exceptions;
using ApacBreachersRanked.Application.Moderation.Models;
using ApacBreachersRanked.Application.Moderation.Queries;
using ApacBreachersRanked.Domain.User.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Moderation.Commands
{
    public class ThrowIfBannedCommand : ICommand
    {
        public IUserId UserId { get; set; } = null!;
    }

    public class ThrowIfBannedHandler : ICommandHandler<ThrowIfBannedCommand>
    {
        private readonly IMediator _mediator;

        public ThrowIfBannedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ThrowIfBannedCommand request, CancellationToken cancellationToken)
        {
            IQueryable<UserBan> activeBans = await _mediator.Send(new GetActiveBansQuery(), cancellationToken);
            UserBan? userBan = await activeBans.OrderBy(x => x.BannedAtUtc)
                .Where(x => x.UserId.Equals(request.UserId))
                .FirstOrDefaultAsync(cancellationToken);

            if (userBan != null)
            {
                throw new UserBannedException(userBan);
            }

            return Unit.Value;
        }
    }
}
