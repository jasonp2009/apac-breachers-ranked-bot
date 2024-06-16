using ApacBreachersRanked.Application.BreachersUsers.Models;
using ApacBreachersRanked.Application.BreachersUsers.Services;
using ApacBreachersRanked.Application.Common.Mediator;

namespace ApacBreachersRanked.Application.BreachersUsers.Queries;

public class GetBreachersUserQuery : IQuery<BreachersUser>
{
    public string BreachersUserId { get; set; }
}

public class GetBreachersUserQueryHandler : IQueryHandler<GetBreachersUserQuery, BreachersUser>
{
    private readonly IBreachersUserService _breachersUserService;

    public GetBreachersUserQueryHandler(IBreachersUserService breachersUserService)
    {
        _breachersUserService = breachersUserService;
    }

    public Task<BreachersUser> Handle(GetBreachersUserQuery request, CancellationToken cancellationToken)
        => _breachersUserService.GetBreachersUser(request.BreachersUserId, cancellationToken);
}
