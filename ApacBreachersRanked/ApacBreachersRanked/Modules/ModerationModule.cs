using ApacBreachersRanked.Application.Moderation.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;

namespace ApacBreachersRanked.Modules
{
    [Group("moderation", "A group of commands for moderating the ranked bot")]
    [DefaultMemberPermissions(Discord.GuildPermission.ModerateMembers)]
    public class ModerationModule : BaseModule
    {
        public ModerationModule(IMediator mediator) : base(mediator)
        {
        }

        [SlashCommand("ban", "Ban a user from ranked")]
        public async Task Ban(
            [Summary("user", "The user you would like to ban")]
            SocketUser user,
            [Summary("duration", "How long you would like to ban the user for, eg 15m, 2h, 3d")]
            TimeSpan duration,
            [Summary("reason", "The reason for banning the user")]
            string reason)
        {
            await DeferAsync(ephemeral: true);
            try
            {
                await _mediator.Send(new BanUserCommand
                {
                    DiscordUserId = user.Id,
                    Duration = duration,
                    Reason = reason
                });
                await Context.Interaction.FollowupAsync("User has been banned");
            }
            catch (Exception ex)
            {
                await Context.Interaction.FollowupAsync(ex.Message);
            }
        }

        [SlashCommand("unban", "Unban a user from ranked")]
        public async Task UnBan(
            [Summary("user", "The user you would like to unban")]
            SocketUser user,
            [Summary("reason", "The reason for unbanning the user")]
            string reason)
        {
            await DeferAsync(ephemeral: true);
            try
            {
                await _mediator.Send(new UnBanUserCommand
                {
                    DiscordUserId = user.Id,
                    Reason = reason
                });
                await Context.Interaction.FollowupAsync("User has been unbanned");
            }
            catch (Exception ex)
            {
                await Context.Interaction.FollowupAsync(ex.Message);
            }
        }

        [SlashCommand("cancelmatch", "Cancel a match")]
        public async Task CancelMatch(
            [Summary("matchNumber", "The match you want to cancel")]
            int matchNumber,
            [Summary("reason", "The reason you want to cancel this match")]
            string cancellationReason)
        {
            await DeferAsync(ephemeral: true);
            try
            {
                await _mediator.Send(new CancelMatchCommand
                {
                    MatchNumber = matchNumber,
                    CancellationReason = cancellationReason
                });
                await Context.Interaction.FollowupAsync("User has been unbanned");
            }
            catch (Exception ex)
            {
                await Context.Interaction.FollowupAsync(ex.Message);
            }
        }
    }
}
