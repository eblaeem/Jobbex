using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Services
{
    public interface IAntiForgeryCookieService
    {
        string RegenerateAntiForgeryCookies(IEnumerable<Claim> claims);
        void DeleteAntiForgeryCookies();
        Task<bool> IsRequestValidAsync(HttpContext httpContext);
        string GetAndStoreTokens();
    }

    public class AntiForgeryCookieService : IAntiForgeryCookieService
    {
        private const string XsrfTokenKey = "XSRF-TOKEN";

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAntiforgery _antiforgery;
        private readonly IOptionsSnapshot<AntiforgeryOptions> _antiforgeryOptions;

        public AntiForgeryCookieService(
            IHttpContextAccessor contextAccessor,
            IAntiforgery antiforgery,
            IOptionsSnapshot<AntiforgeryOptions> antiforgeryOptions)
        {
            _contextAccessor = contextAccessor;
            _antiforgery = antiforgery;
            _antiforgeryOptions = antiforgeryOptions;
        }

        public string RegenerateAntiForgeryCookies(IEnumerable<Claim> claims)
        {
            var httpContext = _contextAccessor.HttpContext;
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
            var tokens = _antiforgery.GetAndStoreTokens(httpContext);
            httpContext.Response.Cookies.Append(
                  key: XsrfTokenKey,
                  value: tokens.RequestToken,
                  options: new CookieOptions
                  {
                      HttpOnly = false 
                  });
            return tokens.RequestToken;
        }

        public void DeleteAntiForgeryCookies()
        {
            var cookies = _contextAccessor.HttpContext.Response.Cookies;
            cookies.Delete(_antiforgeryOptions.Value.Cookie.Name);
            cookies.Delete(XsrfTokenKey);
        }
        public async Task<bool> IsRequestValidAsync(HttpContext httpContext1)
        {
            var httpContext = _contextAccessor.HttpContext;

            var tokens = _antiforgery.GetAndStoreTokens(httpContext);
            await _antiforgery.ValidateRequestAsync(httpContext);

            return false;
        }
        public string GetAndStoreTokens()
        {
            var tokens = _antiforgery.GetAndStoreTokens(_contextAccessor.HttpContext);
            _contextAccessor.HttpContext.Response.Cookies.Append(
                  key: XsrfTokenKey,
                  value: tokens.RequestToken,
                  options: new CookieOptions
                  {
                      HttpOnly = false // Now JavaScript is able to read the cookie
                  });
            return tokens.RequestToken;
        }

    }
}