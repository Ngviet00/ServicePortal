namespace ServicePortals.Application.Dtos.User.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string? UserCode { get; set; }
        public string? Password { get; set; }
        public bool IsChangePassword { get; set; }
        public bool IsActive { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public HashSet<string> Roles { get; set; } = [];
        public HashSet<string> Permissions { get; set; } = [];
    }
}