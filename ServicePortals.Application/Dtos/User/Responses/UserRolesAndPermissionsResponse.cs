namespace ServicePortals.Application.Dtos.User.Responses
{
    public class UserRolesAndPermissionsResponse
    {
        public HashSet<string> Roles { get; set; } = [];
        public HashSet<string> Permissions { get; set; } = [];
    }
}
