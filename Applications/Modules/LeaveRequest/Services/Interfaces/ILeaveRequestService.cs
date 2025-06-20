using System.Security.Claims;
using ServicePortal.Applications.Modules.LeaveRequest.DTO;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Responses;
using ServicePortal.Common;

namespace ServicePortal.Applications.Modules.LeaveRequest.Services.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<PagedResults<LeaveRequestDto>> GetAll(GetAllLeaveRequestDto request);
        Task<LeaveRequestDto> GetById(Guid id);
        Task<LeaveRequestDto?> Approval(ApprovalDto request, string currentUserCode);
        Task<LeaveRequestDto> Create(LeaveRequestDto dto);
        Task<LeaveRequestDto> Update(Guid id, LeaveRequestDto dto);
        Task<LeaveRequestDto> Delete(Guid id);
        Task<PagedResults<LeaveRequestDto>> GetAllWaitApproval(GetAllLeaveRequestWaitApprovalDto request, ClaimsPrincipal userClaim);
        Task<int> CountWaitApproval(GetAllLeaveRequestWaitApprovalDto request, ClaimsPrincipal userClaim);
        Task<string> HrRegisterAllLeave(HrRegisterAllLeaveRqDto request);
        Task<PagedResults<HistoryLeaveRequestApprovalResponse>> GetHistoryLeaveRequestApproval(GetAllLeaveRequestDto request);
        Task<object> CreateLeaveForManyPeople(CreateLeaveRequestForManyPeopleRequest request);
    }
}
