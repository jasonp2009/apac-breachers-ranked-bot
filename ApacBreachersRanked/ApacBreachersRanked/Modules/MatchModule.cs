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

        [SlashCommand("confirm", "Confirm you are ready to play in the match")]
        public async Task Confirm()
        {
            try
            {
                await DeferAsync();
                await _mediator.Send(new PlayerConfirmMatchCommand
                {
                    DiscordUserId = Context.User.Id
                });
                await DeleteOriginalResponseAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        [SlashCommand("enterscore", "Enter scores for match")]
        public async Task EnterScores(
            [Summary("Map")] Map map,
            [Summary("Home"), Autocomplete(typeof(ScoreAutoCompleteHandler))] int home,
            [Summary("Away"), Autocomplete(typeof(ScoreAutoCompleteHandler))] int away)
        {
            try
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
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
