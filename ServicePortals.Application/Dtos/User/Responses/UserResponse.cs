namespace ServicePortals.Application.Dtos.User.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string? UserCode { get; set; }
        public string? Password { get; set; }
        public byte? IsChangePassword { get; set; }
        public byte? IsActive { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
    }
}