﻿using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchQueue.Queries;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using ApacBreachersRanked.Domain.MatchQueue.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchQueue.EventHandlers
{
    public class RequeueConfirmedPlayersHandler : INotificationHandler<MatchCancelledEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IMediator _mediator;

        public RequeueConfirmedPlayersHandler(IDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }
        public async Task Handle(MatchCancelledEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches
                .AsNoTracking()
                .SingleAsync(x => x.Id == notification.MatchId, cancellationToken);

            MatchQueueEntity matchQueue = await _dbContext.MatchQueue
                .AsNoTracking()
                .SingleAsync(x => x.Match != null && x.Match.Id == notification.MatchId, cancellationToken);

            MatchQueueEntity currentQueue = await _mediator.Send(new GetCurrentQueueQuery(), cancellationToken);

            foreach (MatchPlayer? player in match.AllPlayers.Where(player => player.Confirmed))
            {
                MatchQueueUser? user = matchQueue.Users.FirstOrDefault(u => u.UserId.Equals(player.UserId));

                if (user == null) continue;

                DateTime expiryUtc = DateTime.Now.AddMinutes(15);

                if (user.ExpiryUtc > expiryUtc) expiryUtc = user.ExpiryUtc;

                currentQueue.AddUserToQueue(player, expiryUtc);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}