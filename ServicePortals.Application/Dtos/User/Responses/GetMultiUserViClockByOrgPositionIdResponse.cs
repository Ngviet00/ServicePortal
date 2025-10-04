using ServicePortals.Application.Dtos.User.Requests;

namespace ServicePortals.Application.Dtos.User.Responses
{
    public class GetMultiUserViClockByOrgPositionIdResponse
    {
        public int? NVMa { get; set; }
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public int? OrgPositionId { get; set; }
        public string? Email { get; set; }
    }
}
