namespace ServicePortals.Application.Dtos.TimeKeeping.Responses
{
    public class GetUserTimeKeepingResponse
    {
        public int? BCMaNV { get; set; }
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public string? BPTen { get; set; }
        public string? Thu { get; set; }
        public string? BCNgay { get; set; }
        public string? BCGhiChu { get; set; }
        public DateTime? BCTGDen { get; set; }
        public DateTime? BCTGVe { get; set; }
        public string? Results { get; set; }
        public string? CustomResult { get; set; }
        public bool? IsSentToHR { get; set; }
    }
}
