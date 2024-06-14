using System.Net;
using System.Text.Json.Serialization;

namespace ApacBreachersRanked.Infrastructure.Breachers.Models;

public class BreachersApiResponse<T>
{
    public bool Success { get; set; }
    [JsonPropertyName("ErrorCode")]
    public HttpStatusCode StatusCode { get; set; }
    public int TokenError { get; set; }
    public string ErrorMessage { get; set; }
    public IEnumerable<T> Data { get; set; }
}
