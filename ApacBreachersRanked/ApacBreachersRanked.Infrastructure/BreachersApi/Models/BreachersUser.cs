using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.BreachersApi.Models;

public class BreachersUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("user_name")]
    public string UserName { get; set; }
    [JsonPropertyName("id_tag")]
    public int IdTag { get; set; }
    [JsonPropertyName("clan_tag")]
    public string ClanTag { get; set; }
}
