using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Moderation.Commands;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.Match.EventHandlers
{
    public class AutoCancelMatchHandler : INotificationHandler<AutoCancelMatchEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IMediator _mediator;
        
        public AutoCancelMatchHandler(IDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }
        public async Task Handle(AutoCancelMatchEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches
                .Include(x => x.AllPlayers)
                .SingleAsync(match => match.Id == notification.MatchId);

            match.AutoCancel();

            foreach (MatchPlayer unconfirmedPlayer in match.AllPlayers.Where(x => !x.Confirmed))
            {
                await _mediator.Send(new BanUserCommand
                {
                    DiscordUserId = unconfirmedPlayer.UserId.GetDiscordId(),
                    Duration = TimeSpan.FromMinutes(15),
                    Reason = $"Failed to confirm match #{match.MatchNumber} within time limit"
                });
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
