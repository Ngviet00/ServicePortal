namespace ServicePortals.Application.Dtos.User.Responses
{
    public class PersonalInfoResponse
    {
        public string? UserCode { get; set; }
        public bool? IsChangePassword { get; set; }
        public bool? IsActive { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string? NVHoTen { get; set; }
        public int? OrgUnitID { get; set; }
        public int? BPMa { get; set; }
        public string? BPTen { get; set; }
        public string? CVTen { get; set; }
        public bool? NVGioiTinh { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTime? NVNgayVao { get; set; }
    }
}
