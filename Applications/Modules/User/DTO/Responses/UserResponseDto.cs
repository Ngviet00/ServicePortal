namespace ServicePortal.Applications.Modules.User.DTO.Responses
{
    public class UserResponseDto
    {
        public Guid? Id { get; set; }
        public string? UserCode { get; set; }
        public string? Password { get; set; }
        public int? PositionId { get; set; }
        public byte? IsChangePassword { get; set; }
        public byte? IsActive { get; set; }
        public string? Email { get; set; }
        public List<Domain.Entities.Role> Roles { get; set; } = new();
        public bool? IsCheckedHaveManageUserTimeKeeping { get; set; }
    }
}