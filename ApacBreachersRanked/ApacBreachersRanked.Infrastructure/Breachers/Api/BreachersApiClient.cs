using System.Net.Http.Json;
using ApacBreachersRanked.Infrastructure.Breachers.Models;
using ApacBreachersRanked.Infrastructure.Config;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Infrastructure.Breachers.Api;

public class BreachersApiClient
{
    private readonly HttpClient _httpClient;

    public BreachersApiClient(HttpClient httpClient, IOptions<BreachersApiOptions> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new(options.Value.BaseUrl);
    }

    public Task<IEnumerable<BreachersUser>> SearchUsers(string userName, CancellationToken cancellationToken = default)
        => SearchUsers(new List<string> { userName });
    public async Task<IEnumerable<BreachersUser>> SearchUsers(IEnumerable<string> userNames, CancellationToken cancellationToken = default)
    {
        userNames = userNames.Select(userName =>
            userName.Contains(']')
                ? userName.Split(']', 2).LastOrDefault()
                : userName
        );
        string request = $"get_player_id?player_name={string.Join(';', userNames)}";
        BreachersApiResponse<GetUsersResponse> response =
            await _httpClient.GetFromJsonAsync<BreachersApiResponse<GetUsersResponse>>(request, cancellationToken);
        if (!response.Success)
        {
            if (response.ErrorMessage.Equals("No valid names provided") ||
                response.ErrorMessage.Equals("No users found."))
            {
                return new List<BreachersUser>();
            }
            throw new InvalidOperationException($"[{nameof(SearchUsers)}] The breachers API threw an error: {response.ErrorMessage}");
        }
        return response.Data.Single().Users;
    }

    public async Task<IEnumerable<GetMatchResponse>> GetMatchesByUserId(string userId, CancellationToken cancellationToken = default)
    {
        string request = $"get_match_data?player_id={userId}";

        BreachersApiResponse<GetMatchResponse> response =
            await _httpClient.GetFromJsonAsync<BreachersApiResponse<GetMatchResponse>>(request, cancellationToken);
        if (!response.Success)
            throw new InvalidOperationException($"[{nameof(GetMatchesByUserId)}] The breachers API threw an error: {response.ErrorMessage}");
        return response.Data;
    }

    public async Task<BreachersUserStats> GetUserStats(string userId, CancellationToken cancellationToken = default)
    {
        string request = $"get_player_stats?player_id={userId}";
        BreachersApiResponse<BreachersUserStats> response =
            await _httpClient.GetFromJsonAsync<BreachersApiResponse<BreachersUserStats>>(request, cancellationToken);
        if (!response.Success)
            throw new InvalidOperationException($"[{nameof(GetUserStats)}] The breachers API threw an error: {response.ErrorMessage}");
        return response.Data.Single();
    }
}
