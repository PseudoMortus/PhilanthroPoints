using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using PhilanthroPoints.Models;

namespace PhilanthroPoints.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        public Task MarkUserAsAuthenticated(Member member)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, member.Username ?? ""),
                new Claim(ClaimTypes.NameIdentifier, member.Id.ToString())
            };
            
            // For admin users, always assign Admin role (this method is called from admin login)
            // We can determine this is an admin login based on the context
            var identity = new ClaimsIdentity(claims, "CustomAuth");
            _currentUser = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            return Task.CompletedTask;
        }

        public Task MarkAdminAsAuthenticated(Member member)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, member.Username ?? ""),
                new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, "CustomAuth");
            _currentUser = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            return Task.CompletedTask;
        }

        public Task MarkUserAsLoggedOut()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            return Task.CompletedTask;
        }
    }
}
