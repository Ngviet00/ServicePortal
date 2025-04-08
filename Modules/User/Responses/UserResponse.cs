namespace ServicePortal.Modules.User.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Deparment { get; set; }
        public string? Title { get; set; }
        public int Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
