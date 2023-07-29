using ApacBreachersRanked.Application.Match.Commands;
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

        }

        [SlashCommand("enterscore", "Enter scores for match")]
        public async Task EnterScores(
            [Summary("Map")] Map map,
            [Summary("Home"), Autocomplete(typeof(ScoreAutoCompleteHandler))] int home,
            [Summary("Away"), Autocomplete(typeof(ScoreAutoCompleteHandler))] int away)
        {
            await _mediator.Send(new EnterMatchScoreCommand
            {
                MatchThreadId = Context.Channel.Id,
                Map = map,
                Home = home,
                Away = away
            });
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
        }
    }
}
