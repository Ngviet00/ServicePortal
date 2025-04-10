using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServicePortal.Domain.Enums;

namespace ServicePortal.Common.Filters
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly HashSet<string> _allowedRoles;

        public RoleAuthorizeAttribute(params RoleEnum[] roles)
        {
            _allowedRoles = roles
                .Select(r => r.ToString())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new JsonResult(new
                {
                    status = 401,
                    message = "Unauthorized"
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            if (string.IsNullOrEmpty(roleClaim))
            {
                context.Result = new JsonResult(new
                {
                    status = 403,
                    message = "Forbidden"
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            if (string.Equals(roleClaim, RoleEnum.SuperAdmin.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (string.IsNullOrEmpty(roleClaim) || !_allowedRoles.Contains(roleClaim))
            {
                context.Result = new JsonResult(new
                {
                    status = 403,
                    message = "Forbidden"
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
