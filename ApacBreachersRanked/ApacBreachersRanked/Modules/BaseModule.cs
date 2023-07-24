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
    }
}
