using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchVote.Models;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Domain.Match.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchVote.Commands
{
    public class CastMapVoteCommand : ICommand
    {
        public int MatchNumber { get; set; }
        public ulong DiscordUserId { get; set; }
        public Map Vote { get; set; }
    }

    public class CastMapVoteCommandHandler : ICommandHandler<CastMapVoteCommand>
    {
        private readonly IDbContext _dbContext;

        public CastMapVoteCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(CastMapVoteCommand request, CancellationToken cancellationToken)
        {
            MatchVoteModel matchVote = await _dbContext.MatchVotes
                .Where(x => x.Match.MatchNumber == request.MatchNumber)
                .SingleAsync(cancellationToken);

            matchVote.CastMapVote(request.DiscordUserId.ToIUserId(), request.Vote);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
