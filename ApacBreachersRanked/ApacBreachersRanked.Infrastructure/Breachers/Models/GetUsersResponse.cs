using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.Breachers.Models;

public class GetUsersResponse
{
    [JsonPropertyName("users")]
    public IEnumerable<BreachersUser> Users { get; set; }
    [JsonPropertyName("invalid_names")]
    public IEnumerable<string> InvalidNames { get; set; }
}
