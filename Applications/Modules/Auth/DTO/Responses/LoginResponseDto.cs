namespace ServicePortal.Applications.Modules.Auth.DTO.Responses
{
    public class LoginResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public object? UserInfo { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
    }
}
