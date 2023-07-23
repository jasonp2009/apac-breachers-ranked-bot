using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Domain.Entities;
using ApacBreachersRanked.Domain.Interfaces;
using Discord;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.Match.Events
{
    public class CreateMatchHandler : INotificationHandler<MatchQueueCapacityReachedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IMatchService _matchService;
        public CreateMatchHandler(
            IDbContext dbContext,
            IMatchService matchService)
        {
            _dbContext = dbContext;
            _matchService = matchService;
        }

        public async Task Handle(MatchQueueCapacityReachedEvent notification, CancellationToken cancellationToken)
        {
            MatchQueueEntity? matchQueue = await _dbContext.MatchQueue.FirstOrDefaultAsync(x => x.Id == notification.MatchQueueId, cancellationToken);
            if (matchQueue == null || !matchQueue.IsOpen) return;

            try
            {
                MatchEntity match = await _matchService.CreateMatchFromQueue(matchQueue);



                _dbContext.Matches.Add(match);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}
