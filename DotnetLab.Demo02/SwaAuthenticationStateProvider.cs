using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotnetLab.Demo02
{
    public class SwaAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;

        public SwaAuthenticationStateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var authenticationData = await _httpClient.GetFromJsonAsync<AuthenticationData>("/.auth/me");
                authenticationData.ClientPrincipal.UserRoles = authenticationData
                    .ClientPrincipal
                    .UserRoles
                    ?.Except(new string[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase);

                if (!authenticationData.ClientPrincipal.UserRoles.Any())
                {
                    // 認証されていないケース
                    return CreateUnauthrizedAuthenticationState();
                }

                var identity = new ClaimsIdentity(authenticationData.ClientPrincipal.IdentityProvider);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, authenticationData.ClientPrincipal.UserId));
                identity.AddClaim(new Claim(ClaimTypes.Name, authenticationData.ClientPrincipal.UserDetails));
                identity.AddClaims(authenticationData.ClientPrincipal.UserRoles
                    .Select(x => new Claim(ClaimTypes.Role, x)));
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                // 認証されていないケース
                return CreateUnauthrizedAuthenticationState();
            }
        }

        private static AuthenticationState CreateUnauthrizedAuthenticationState() =>
            new AuthenticationState(new ClaimsPrincipal(
                new ClaimsIdentity()));
    }

    public class AuthenticationData
    {
        public ClientPrincipal ClientPrincipal { get; set; }
    }

    public class ClientPrincipal
    {
        public string IdentityProvider { get; set; }
        public string UserId { get; set; }
        public string UserDetails { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
    }
}
