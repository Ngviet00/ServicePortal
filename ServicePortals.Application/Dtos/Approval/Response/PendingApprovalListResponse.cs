namespace ServicePortals.Application.Dtos.Approval.Response
{
    public class PendingApproval
    {
        public string? Code { get; set; } //mã đơn
        public int? Type { get; set; } //ex: id = 4,..
        public string? TypeName { get; set; } //ex: Nghỉ phép,..
        public string? Requester { get; set; } //người yêu cầu
        public string? Registrant { get; set; } //người đăng ký
        public string? ApprovedBy { get; set; } //người đã duyệt
        public int? Status { get; set; } //trạng thái
    }

    public class PendingApprovalList
    {
        public int TotalCount { get; set; }
        public List<PendingApproval> Data { get; set; } = [];
    }
}
