using ApacBreachersRanked.Application.Common.Mediator;
using ApacBreachersRanked.Application.DbContext;
using ApacBreachersRanked.Application.Moderation.Models;
using ApacBreachersRanked.Application.Users;
using MediatR;

namespace ApacBreachersRanked.Application.Moderation.Commands
{
    public class UnBanUserCommand : ICommand
    {
        public ulong DiscordUserId { get; set; }
        public string Reason { get; set; } = null!;
    }

    public class UnBanUserHandler : ICommandHandler<UnBanUserCommand>
    {
        private readonly IDbContext _dbContext;
        
        public UnBanUserHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(UnBanUserCommand request, CancellationToken cancellationToken)
        {
            IQueryable<UserBan> usersActiveBans = _dbContext.UserBans
                .Where(x => x.UserId.Equals(request.DiscordUserId.ToIUserId()) &&
                       x.BannedAtUtc <= DateTime.UtcNow && DateTime.UtcNow <= x.ExpiryUtc &&
                       !x.UnBanOverride);
            foreach (UserBan userBan in usersActiveBans)
            {
                userBan.UnBan(request.Reason);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
