namespace ServicePortals.Application.Dtos.User.Requests
{
    public class UpdatePersonalInfoRequest
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
    }
}
