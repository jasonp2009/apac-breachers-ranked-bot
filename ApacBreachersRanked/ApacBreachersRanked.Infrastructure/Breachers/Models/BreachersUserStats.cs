using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.Breachers.Models;

public class BreachersUserStats : BreachersUser
{
    [JsonPropertyName("username")]
    public override string UserName { get; set; }
}
