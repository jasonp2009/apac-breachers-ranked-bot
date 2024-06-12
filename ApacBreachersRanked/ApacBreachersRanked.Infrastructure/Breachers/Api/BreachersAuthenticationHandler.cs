using System.Security.Cryptography;
using System.Text;
using ApacBreachersRanked.Infrastructure.Config;
using Microsoft.Extensions.Options;

namespace ApacBreachersRanked.Infrastructure.Breachers.Api;

public class BreachersAuthenticationHandler : DelegatingHandler
{
    private readonly BreachersApiOptions _options;

    public BreachersAuthenticationHandler(IOptions<BreachersApiOptions> options)
    {
        _options = options.Value;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string queryString = request.RequestUri?.Query
            ?? throw new InvalidOperationException("Unable to get query from request uri");
        var queryValues = queryString.Split('&').Select(param => param.Split('=').LastOrDefault());
        var msg = $"{_options.Token}.{string.Join(',', queryValues)}.{_options.Secret}";
        request.Headers.Add("x-api-token", _options.Token);
        request.Headers.Add("x-api-auth", $"{_options.ApiId}:{GetHash(msg)}");
        return base.SendAsync(request, cancellationToken);
    }

    private string GetHash(string msg)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(msg);
        SHA256Managed hashstring = new SHA256Managed();
        byte[] hash = hashstring.ComputeHash(bytes);
        string hashString = string.Empty;
        foreach (byte x in hash)
        {
            hashString += String.Format("{0:x2}", x);
        }
        return hashString;
    }
}
