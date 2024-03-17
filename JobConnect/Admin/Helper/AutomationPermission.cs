using Microsoft.AspNetCore.Authorization;
using Services;

namespace Web.Services
{
    public class AutomationPermissionRequirement : IAuthorizationRequirement
    {

    }
    public class AutomationAuthorizationHandler : AuthorizationHandler<AutomationPermissionRequirement>
    {
        private readonly ISecurityService _securityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AutomationAuthorizationHandler(ISecurityService securityTrimmingService,
            IHttpContextAccessor httpContextAccessor)
        {
            _securityService = securityTrimmingService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AutomationPermissionRequirement requirement)
        {
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();

            var areaName = routeData?.Values["area"]?.ToString();
            var area = string.IsNullOrEmpty(areaName) ? string.Empty : areaName;

            var controllerName = routeData?.Values["controller"]?.ToString();
            var controller = string.IsNullOrEmpty(controllerName) ? string.Empty : controllerName;

            var actionName = routeData?.Values["action"]?.ToString();
            var action = string.IsNullOrEmpty(actionName) ? string.Empty : actionName;


            var url = "";
            if (string.IsNullOrEmpty(area) == false)
            {
                url = $"/{area}/";
            }
            url += $"{controller}/{action}".Trim().ToLower();
            var response = _securityService.HasUserAccess(1);
            if (response)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
