namespace ServicePortal.Application.DTOs.Auth.Responses
{
    public class LoginResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public object? UserInfo { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
