using ApacBreachersRanked.Application.Match.Commands;
using ApacBreachersRanked.Domain.Match.Events;
using Discord.Interactions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            await _mediator.Send(new PlayerConfirmMatchCommand
            {
                DiscordUserId = Context.User.Id
            });
            await RespondAsync();
        }
    }
}
