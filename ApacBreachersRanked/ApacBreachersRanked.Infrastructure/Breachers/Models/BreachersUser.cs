using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.Breachers.Models;

public class BreachersUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("user_name")]
    public virtual string UserName { get; set; }
    [JsonPropertyName("id_tag")]
    public int IdTag { get; set; }
    [JsonPropertyName("clan_tag")]
    public string ClanTag { get; set; }

    public string GetFullUserName()
    {
        string fullUserName = "";
        fullUserName += !string.IsNullOrWhiteSpace(ClanTag)
            ? $"[{ClanTag}]"
            : "";
        fullUserName += UserName;
        return fullUserName;
    }
}
