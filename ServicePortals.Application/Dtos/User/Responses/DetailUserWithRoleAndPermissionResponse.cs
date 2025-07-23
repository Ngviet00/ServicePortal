namespace ServicePortals.Application.Dtos.User.Responses
{
    public class DetailUserWithRoleAndPermissionResponse
    {
        public string? UserCode { get; set; }
        public List<Domain.Entities.Role> Roles { get; set; } = [];
        public List<Domain.Entities.Permission> Permissions { get; set; } = [];
    }
}
