using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ServicePortal.Common.Filters
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly HashSet<string> _allowedRoles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _allowedRoles = roles
                .Select(r => r.Trim())
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
                return;
            }

            var roleClaims = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (roleClaims.Count == 0)
            {
                context.Result = new JsonResult(new
                {
                    status = 403,
                    message = "Bạn không có quyền truy cập, hãy liên hệ team IT"
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            if (roleClaims.Contains("superadmin"))
            {
                return;
            }

            if (!_allowedRoles.Overlaps(roleClaims))
            {
                context.Result = new JsonResult(new
                {
                    status = 403,
                    message = "Bạn không có quyền truy cập, hãy liên hệ team IT"
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
