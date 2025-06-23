namespace ServicePortals.Application.Dtos.User.Responses
{
    public class GetEmailByUserCodeAndUserConfigResponse
    {
        public string? UserCode { get; set; }
        public string? Email { get; set; }
        public string? ConfigKey { get; set; }
        public string? ConfigValue { get; set; }
    }
}
