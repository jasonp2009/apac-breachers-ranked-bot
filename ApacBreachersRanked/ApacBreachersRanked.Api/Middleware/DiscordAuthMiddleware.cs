using ApacBreachersRanked.Api.Attributes;
using ApacBreachersRanked.Application.Users.Services;
using Discord;
using Discord.Rest;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace ApacBreachersRanked.Api.Middleware;

public class DiscordAuthMiddleware
{
    private readonly RequestDelegate _next;

    public DiscordAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out StringValues token))
        {
            await using var client = new DiscordRestClient();
            await client.LoginAsync(TokenType.Bearer, token.ToString().Split(' ').LastOrDefault());
            var userContextService = context.RequestServices.GetRequiredService<DiscordUserContextService>();
            userContextService.SetUserContext(client.CurrentUser);
        }
        else
        {
            if (context.GetEndpoint()?.Metadata.Any(m => m is RequireDiscordAuthAttribute) ?? false)
            {
                throw new UnauthorizedAccessException("Please login with discord.");
            }
        }

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}
