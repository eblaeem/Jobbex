using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Services
{
    public interface ICookieValidatorService
    {
        Task ValidateAsync(CookieValidatePrincipalContext context);
    }

    public class CookieValidatorService : ICookieValidatorService
    {
        private readonly IUsersService _usersService;
        public CookieValidatorService(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                await handleUnauthorizedRequest(context);
                return;
            }

            var serialNumberClaim = claimsIdentity.FindFirst(ClaimTypes.SerialNumber);
            if (serialNumberClaim == null)
            {
                await handleUnauthorizedRequest(context);
                return;
            }

            var userIdString = claimsIdentity.FindFirst(ClaimTypes.UserData).Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                await handleUnauthorizedRequest(context);
                return;
            }

            var user = await _usersService.FindUser(userId).ConfigureAwait(false);
            if (user == null || user.SerialNumber != serialNumberClaim.Value || !user.IsActive)
            {
                await handleUnauthorizedRequest(context);
            }

            await _usersService.UpdateUserLastActivityDate(userId).ConfigureAwait(false);
        }

        private Task handleUnauthorizedRequest(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            return context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
