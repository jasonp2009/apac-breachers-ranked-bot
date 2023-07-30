using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Match.Entities;
using ApacBreachersRanked.Domain.Match.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.Match.EventHandlers
{
    public class AutoCancelMatchHandler : INotificationHandler<AutoCancelMatchEvent>
    {
        private readonly IDbContext _dbContext;
        
        public AutoCancelMatchHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(AutoCancelMatchEvent notification, CancellationToken cancellationToken)
        {
            MatchEntity match = await _dbContext.Matches.SingleAsync(match => match.Id == notification.MatchId);

            match.AutoCancel();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
