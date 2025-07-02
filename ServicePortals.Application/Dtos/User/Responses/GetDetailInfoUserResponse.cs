namespace ServicePortals.Application.Dtos.User.Responses
{
    public class GetDetailInfoUserResponse
    {
        public dynamic? InfoFromViclock { get; set; }
        public UserResponse? User { get; set; }
        public HashSet<string> Roles { get; set; } = [];
        public HashSet<string> Permissions { get; set; } = [];
    }
}
