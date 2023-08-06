using ApacBreachersRanked.Application.Match.Commands;
using ApacBreachersRanked.Application.MMR.Commands;
using ApacBreachersRanked.AutoCompleteHandlers;
using ApacBreachersRanked.Domain.Match.Enums;
using Discord.Interactions;
using MediatR;

namespace ApacBreachersRanked.Modules
{
    public class MatchModule : BaseModule
    {
        public MatchModule(IMediator mediator) : base(mediator)
        {
        }

        [ComponentInteraction("pending-match-*")]
        public async Task RespondMatch(string response)
        {
            await _mediator.Send(new PlayerRespondMatchCommand
            {
                DiscordUserId = Context.User.Id,
                IsAccepted = response == "confirm"
            });
            await DeferAsync();
        }

        [SlashCommand("enterscore", "Enter scores for match")]
        public async Task EnterScores(
            [Summary("Map")] Map map,
            [Summary("Home"), Autocomplete(typeof(ScoreAutoCompleteHandler))] int home,
            [Summary("Away"), Autocomplete(typeof(ScoreAutoCompleteHandler))] int away)
        {
            await DeferAsync();
            await _mediator.Send(new EnterMatchScoreCommand
            {
                MatchThreadId = Context.Channel.Id,
                Map = map,
                Home = home,
                Away = away
            });
            await DeleteOriginalResponseAsync();
        }

        [ComponentInteraction("pending-score-*")]
        public async Task RespondScore(string response)
        {
            if (Context.Interaction is Discord.WebSocket.SocketMessageComponent interaction)
            {
                await _mediator.Send(new PlayerRespondScoreCommand
                {
                    IsConfirm = response == "confirm",
                    PendingScoreMessageId = interaction.Message.Id,
                    UserId = interaction.User.Id
                });
            }
            await DeferAsync();
        }

        [SlashCommand("recalculatemmr", "recalc")]
        [RequireOwner]
        public async Task RecalculateMMR()
        {
            await DeferAsync(ephemeral: true);
            try
            {
                await _mediator.Send(new RecalculateMMRCommand());
                await Context.Interaction.FollowupAsync("MMR Recalculated", ephemeral: true);
            }
            catch (Exception ex)
            {
                await Context.Interaction.FollowupAsync(ex.Message, ephemeral: true);
                throw;
            }
        }
    }
}
