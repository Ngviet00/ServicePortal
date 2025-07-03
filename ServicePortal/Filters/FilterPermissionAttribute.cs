using Microsoft.AspNetCore.Mvc.Filters;

namespace ServicePortal.Filters
{
    public class FilterPermissionAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
