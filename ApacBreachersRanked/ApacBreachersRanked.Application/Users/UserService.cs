using ApacBreachersRanked.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacBreachersRanked.Application.Users
{
    internal class UserService : IUserService
    {
        private readonly IMediator _mediator;
        public UserService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IUser> GetUserAsync(IUserId userId)
        {
            if (userId is ApplicationDiscordUserId typedUserId)
            {
                return await _mediator.Send(new GetDiscordUserQuery() { DiscordUserId = typedUserId.Id });
            }
            throw new InvalidOperationException($"Invalid userId type {userId.GetType()}");
        }
    }
}
