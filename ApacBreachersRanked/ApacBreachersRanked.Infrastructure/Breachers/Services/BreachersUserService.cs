using ApacBreachersRanked.Application.BreachersUsers.Services;
using ApacBreachersRanked.Application.Users;
using ApacBreachersRanked.Infrastructure.Breachers.Api;
using ApacBreachersRanked.Infrastructure.Breachers.Entities;
using ApacBreachersRanked.Infrastructure.Breachers.Models;
using ApacBreachersRanked.Infrastructure.Persistance;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApplicationBreachersUser = ApacBreachersRanked.Application.BreachersUsers.Models.BreachersUser;

namespace ApacBreachersRanked.Infrastructure.Breachers.Services;

internal class BreachersUserService : IBreachersUserService
{
    private readonly BreachersApiClient _breachersApiClient;
    private readonly IMapper _mapper;
    private readonly BreachersDbContext _dbContext;

    public BreachersUserService(BreachersApiClient breachersApiClient, IMapper mapper, BreachersDbContext dbContext)
    {
        _breachersApiClient = breachersApiClient;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ApplicationBreachersUser>> SearchUsers(string searchString, CancellationToken cancellationToken)
    {
        IEnumerable<BreachersUser> users = await _breachersApiClient.SearchUsers(searchString, cancellationToken);
        return users.Select(user => _mapper.Map<ApplicationBreachersUser>(user));
    }

    public async Task LinkDiscordUser(ApplicationDiscordUser discordUser, string breachersUserId, CancellationToken cancellationToken)
    {
        BreachersDiscordUserLink existingBreachersUserLink =
            await _dbContext.BreachersDiscordUserLinks.FirstOrDefaultAsync(x =>
                x.DiscordUserId == discordUser.UserId.GetDiscordId(), cancellationToken);
        if (existingBreachersUserLink != null)
        {
            BreachersUser existingBreachersUser = await _breachersApiClient.GetUserStats(breachersUserId, cancellationToken);
            throw new InvalidOperationException(
                $"Discord user {discordUser.Name} is already linked to breachers user {existingBreachersUser.GetFullUserName()}");
        }

        BreachersUser breachersUser =
            await _breachersApiClient.GetUserStats(breachersUserId, cancellationToken);
        if (breachersUser == null)
        {
            throw new InvalidOperationException("Invalid breachers user id");
        }
        BreachersDiscordUserLink newBreachersUserLink = new()
        {
            DiscordUserId = discordUser.UserId.GetDiscordId(),
            BreachersUserId = breachersUserId
        };
        await _dbContext.BreachersDiscordUserLinks.AddAsync(newBreachersUserLink, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ApplicationBreachersUser> GetBreachersUser(ApplicationDiscordUser discordUser, CancellationToken cancellationToken)
    {
        BreachersDiscordUserLink userLink =
            await _dbContext.BreachersDiscordUserLinks.FirstOrDefaultAsync(
                x => x.DiscordUserId == discordUser.UserId.GetDiscordId(), cancellationToken);
        if (userLink == null) return null;

        BreachersUser breachersUser =
            await _breachersApiClient.GetUserStats(userLink.BreachersUserId, cancellationToken);
        return _mapper.Map<ApplicationBreachersUser>(breachersUser);
    }
}
