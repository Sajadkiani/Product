using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Identity.Api.Infrastructure.Consts;
using Identity.Infrastructure.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Product.Infrastructure.Clients.Grpc;
using Product.Infrastructure.Models;
using Product.Infrastructure.Utils;

namespace Product.Api.Security;

public class AppAuthenticationHandler : AuthenticationHandler<AppOptions.Jwt>
{
    private readonly IMemoryCache cache;
    private readonly ICurrentUser currentUser;
    private readonly IIdentityClient identityClient;

    public AppAuthenticationHandler(
        IOptionsMonitor<AppOptions.Jwt> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IMemoryCache cache,
        ICurrentUser currentUser,
        IIdentityClient identityClient
    ) : base(options, logger, encoder, clock)
    {
        this.cache = cache;
        this.currentUser = currentUser;
        this.identityClient = identityClient;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string authorization = Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorization))
        {
            return AuthenticateResult.NoResult();
        }

        string refreshToken = string.Empty;

        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            refreshToken = authorization.Substring("Bearer ".Length).Trim();
        }

        if (string.IsNullOrEmpty(refreshToken))
        {
            return AuthenticateResult.Fail("Invalid authenticate header");
        }

        var token = cache.Get<AuthModel.GetTokenModel>(CacheKeys.Token + refreshToken);
        if (token is null)
        {
            token = await RefreshTokenAsync(refreshToken);
            if (token is null)
                return AuthenticateResult.Fail("Invalid authenticate header");
            
            cache.Set(CacheKeys.Token + refreshToken, token);
        }

        if (string.IsNullOrWhiteSpace(token.AccessToken))
            return AuthenticateResult.Fail("Token not found");

        try
        {
            var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token.AccessToken,
                Options.TokenValidationParameters, out _);

            currentUser.Set(claimsPrincipal);

            var ticket = new AuthenticationTicket(claimsPrincipal, AppOptions.Jwt.Scheme);
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception)
        {
            return AuthenticateResult.Fail("Invalid access token");
        }
    }

    public async Task<AuthModel.GetTokenModel> RefreshTokenAsync(string refreshToken)
    {
        var token = await identityClient.RefreshTokenAsync(refreshToken);
        return token;
    }
}