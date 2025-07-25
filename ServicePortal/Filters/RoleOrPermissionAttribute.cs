using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ServicePortal.Filters
{
    public class RoleOrPermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly HashSet<string> _allowedStrings;
        public RoleOrPermissionAttribute(params string[] allowedStrings)
        {
            _allowedStrings = allowedStrings.Select(s => s.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user?.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new JsonResult(new { status = 401, message = "Unauthorized" }) { StatusCode = 401 };
                return;
            }

            var roleClaims = user?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            var permissionClaims = user?.Claims.Where(c => c.Type == "permission").Select(c => c.Value);

            // Trường hợp user là superadmin
            if (roleClaims != null && roleClaims.Any(r => r.Equals("superadmin", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            bool hasAccess = _allowedStrings.Any(s => roleClaims != null && roleClaims.Contains(s, StringComparer.OrdinalIgnoreCase))
                             || _allowedStrings.Any(s => permissionClaims != null && permissionClaims.Contains(s, StringComparer.OrdinalIgnoreCase));

            if (!hasAccess)
            {
                context.Result = new JsonResult(new { status = 403, message = "Bạn không có quyền truy cập, hãy liên hệ team IT" }) { StatusCode = 403 };
            }
        }
    }
}
