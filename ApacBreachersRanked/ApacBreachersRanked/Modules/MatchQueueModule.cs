using ApacBreachersRanked.Application.MatchQueue.Commands;
using Discord.Interactions;
using MediatR;

namespace ApacBreachersRanked.Modules
{
    public class MatchQueueModule : BaseModule
    {

        public MatchQueueModule(IMediator mediator)
            : base(mediator)
        {
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
                await DeleteOriginalResponseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        [SlashCommand("forcematch", "Force a match to start")]
        [RequireRole("Developers")]
        public async Task ForceMatchAsync()
        {
            try
            {
                await _mediator.Send(new ForceMatchCommand());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
