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

    public Task<IEnumerable<BreachersUser>> SearchUsers(string userName)
        => SearchUsers(new List<string> { userName });
    public async Task<IEnumerable<BreachersUser>> SearchUsers(IEnumerable<string> userNames)
    {
        string request = $"get_player_id?player_name={string.Join(';', userNames)}";
        BreachersApiResponse<GetUsersResponse> response = await _httpClient.GetFromJsonAsync<BreachersApiResponse<GetUsersResponse>>(request);
        if (!response.Success)
            throw new InvalidOperationException($"The breachers API threw an error: {response.ErrorMessage}");
        return response.Data.Single().Users;
    }

    public async Task<IEnumerable<GetMatchResponse>> GetMatchesByUserId(string userId)
    {
        string request = $"get_match_data?player_id={userId}";
        
        BreachersApiResponse<GetMatchResponse> response = await _httpClient.GetFromJsonAsync<BreachersApiResponse<GetMatchResponse>>(request);
        if (!response.Success)
            throw new InvalidOperationException($"The breachers API threw an error: {response.ErrorMessage}");
        return response.Data;
    }
}
