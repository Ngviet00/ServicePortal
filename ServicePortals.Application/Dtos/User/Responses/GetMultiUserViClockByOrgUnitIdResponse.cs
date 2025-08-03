using ServicePortals.Application.Dtos.User.Requests;

namespace ServicePortals.Application.Dtos.User.Responses
{
    public class GetMultiUserViClockByOrgUnitIdResponse
    {
        public int? NVMa { get; set; }
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public int? OrgUnitID { get; set; }
        public string? Email { get; set; }
    }

    public class GetMultiUserViClockByOrgUnitIdConfirmTimeKeepingResponse
    {
        public int? NVMa { get; set; }
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public int? OrgUnitID { get; set; }
        public string? BPTen { get; set; }
        public DateTime? NVNgayVao { get; set; }
    }
}
