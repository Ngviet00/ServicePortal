namespace ServicePortal.Modules.User.DTO.Requests
{
    public class ResetPasswordDto
    {
        public string? UserCode {  get; set; }
        public string? Password {  get; set; }
    }
}
