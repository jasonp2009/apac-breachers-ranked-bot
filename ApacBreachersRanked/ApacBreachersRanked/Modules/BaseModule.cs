using Discord.Interactions;
using MediatR;

namespace ApacBreachersRanked.Modules
{
    public abstract class BaseModule : InteractionModuleBase<SocketInteractionContext>
    {
        protected readonly IMediator _mediator;

        public BaseModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task BeforeExecuteAsync(ICommandInfo command)
        {
            await DeferAsync();
        }

        public override async Task AfterExecuteAsync(ICommandInfo command)
        {
            if (Context.Interaction.Type == Discord.InteractionType.ApplicationCommand)
            {
                await DeleteOriginalResponseAsync();
            }
        }
    }
}
