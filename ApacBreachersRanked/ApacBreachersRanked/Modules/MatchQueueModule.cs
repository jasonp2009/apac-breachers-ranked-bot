using ApacBreachersRanked.Application.MatchQueue;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Modules
{
    public class MatchQueueModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator;

        public MatchQueueModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SlashCommand("joinqueue", "Join the ranked queue")]
        public async Task JoinQueueAsync(int timeoutMins = 60)
        {
            try
            {
                AddUserToQueueCommand command = new AddUserToQueueCommand()
                {
                    DiscordUserId = Context.User.Id,
                    TimeoutMins = timeoutMins
                };
                await _mediator.Send(command);
                await RespondAsync("You have joined the queue");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}
