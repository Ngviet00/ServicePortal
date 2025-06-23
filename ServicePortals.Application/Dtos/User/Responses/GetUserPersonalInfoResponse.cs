namespace ServicePortals.Application.Dtos.User.Responses
{
    public class GetUserPersonalInfoResponse
    {
        public string? NVMaNV { get; set; }
        public int? NVMaBP { get; set; }
        public string? BPTen { get; set; }
        public int? NVMaCV { get; set; }
        public string? CVTen { get; set; }
        public string? NVHoTen { get; set; }
        public bool? NVGioiTinh { get; set; }
        public DateTime? NVNgaySinh { get; set; }
        public DateTime? NVNgayVao { get; set; }
        public string? NVDienThoai { get; set; }
        public string? NVEmail { get; set; }
    }
}
