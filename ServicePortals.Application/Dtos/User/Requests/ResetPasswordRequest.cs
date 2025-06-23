namespace ServicePortals.Application.Dtos.User.Requests
{
    public class ResetPasswordRequest
    {
        public string? UserCode {  get; set; }
        public string? Password {  get; set; }
    }
}
