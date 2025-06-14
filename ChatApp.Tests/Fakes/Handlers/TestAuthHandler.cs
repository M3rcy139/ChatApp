using System.Security.Claims;
using System.Text.Encodings.Web;
using ChatApp.Tests.Fakes.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatApp.Tests.Fakes.Handlers;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public static CurrentUserProvider _currentUserProvider;

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        CurrentUserProvider currentUserProvider)
        : base(options, logger, encoder, clock)
    {
        _currentUserProvider = currentUserProvider;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, _currentUserProvider.CurrentUserId.ToString()), 
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
