using ApacBreachersRanked.Application.BreachersUsers.Models;
using ApacBreachersRanked.Application.BreachersUsers.Services;
using ApacBreachersRanked.Application.Common.Mediator;

namespace ApacBreachersRanked.Application.BreachersUsers.Queries;

public class SearchBreachersUsersQuery : IQuery<IEnumerable<BreachersUser>>
{
    public string SearchString { get; set; }
}

public class SearchBreachersUsersQueryHandler :
    IQueryHandler<SearchBreachersUsersQuery, IEnumerable<BreachersUser>>
{
    private readonly IBreachersUserService _breachersUserService;

    public SearchBreachersUsersQueryHandler(IBreachersUserService breachersUserService)
    {
        _breachersUserService = breachersUserService;
    }

    public Task<IEnumerable<BreachersUser>> Handle(SearchBreachersUsersQuery request,
        CancellationToken cancellationToken)
        => _breachersUserService.SearchUsers(request.SearchString, cancellationToken);
}
