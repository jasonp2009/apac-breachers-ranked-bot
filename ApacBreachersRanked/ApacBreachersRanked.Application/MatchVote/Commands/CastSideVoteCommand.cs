using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.MatchVote.Enums;
using ApacBreachersRanked.Application.MatchVote.Models;
using ApacBreachersRanked.Application.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApacBreachersRanked.Application.MatchVote.Commands
{
    public class CastSideVoteCommand : ICommand
    {
        public int MatchNumber { get; set; }
        public ulong DiscordUserId { get; set; }
        public GameSide Vote { get; set; }
    }

    public class CastSideVoteCommandHandler : ICommandHandler<CastSideVoteCommand>
    {
        private readonly IDbContext _dbContext;

        public CastSideVoteCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(CastSideVoteCommand request, CancellationToken cancellationToken)
        {
            MatchVoteModel matchVote = await _dbContext.MatchVotes.SingleAsync(x => x.Match.MatchNumber == request.MatchNumber, cancellationToken);

            matchVote.CastSideVote(request.DiscordUserId.ToIUserId(), request.Vote);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
