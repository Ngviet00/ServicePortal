﻿namespace ServicePortal.Applications.Modules.User.DTO.Responses
{
    public class GetAllUserResponseDto
    {
        public Guid Id { get; set; }
        public string? UserCode { get; set; }
        public string? NVHoTen { get; set; }
        public int? NVMaBP { get; set; }
        public string? BPTen { get; set; }
        public string? CVTen { get; set; }
        public int? NVMaCV { get; set; }
        public bool? NVGioiTinh { get; set; }
        public string? NVDienThoai { get; set; }
        public string? NVEmail { get; set; }
        public DateTime? NVNgayVao { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public List<Domain.Entities.Role>? Roles { get; set; }
    }
}
