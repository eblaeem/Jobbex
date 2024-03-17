using Microsoft.AspNetCore.Antiforgery;

namespace Api.Helper
{
    public class AntiforgeryTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAntiforgery _antiforgery;

        public AntiforgeryTokenMiddleware(RequestDelegate next, IAntiforgery antiforgery)
        {
            _next = next;
            _antiforgery = antiforgery;
        }
        public Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (path != "/" && !path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) && context.Request.Method=="GET")
            {
                var tokens = _antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append(
                      key: "XSRF-TOKEN",
                      value: tokens.RequestToken,
                      options: new CookieOptions
                      {
                          HttpOnly = false 
                      });
            }
            return _next(context);
        }
    }

    public static class AntiforgeryTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseAntiforgeryToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiforgeryTokenMiddleware>();
        }
    }
}
