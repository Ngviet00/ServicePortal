using System.Security.Claims;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.LeaveRequest;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.LeaveRequest.Responses;

namespace ServicePortals.Application.Interfaces.LeaveRequest
{
    public interface ILeaveRequestService
    {
        Task<PagedResults<LeaveRequestDto>> GetAll(GetAllLeaveRequest request);
        Task<Domain.Entities.LeaveRequest> GetById(Guid id);
        Task<LeaveRequestDto?> Approval(ApprovalRequest request);
        Task<LeaveRequestDto> Create(CreateLeaveRequest request, ClaimsPrincipal userClaim);
        Task<LeaveRequestDto> Update(Guid id, LeaveRequestDto dto);
        Task<LeaveRequestDto> Delete(Guid id);
        Task<object> UpdateUserHavePermissionCreateMultipleLeaveRequest(List<string> UserCodes);
        Task<object> GetUserCodeHavePermissionCreateMultipleLeaveRequest();

        Task<object> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request);
        Task<object> GetOrgUnitIdAttachedByUserCode(string userCode);

        Task<object> CreateLeaveForManyPeople(CreateLeaveRequestForManyPeopleRequest request);
        Task<object> SearchUserRegisterLeaveRequest(SearchUserRegisterLeaveRequest request);
        Task<object> HrRegisterAllLeave(HrRegisterAllLeaveRequest request);

        Task<List<HrMngLeaveRequestResponse>> GetHrWithManagementLeavePermission();
        Task<object> UpdateHrWithManagementLeavePermission(List<string> UserCode);
        Task<byte[]> HrExportExcelLeaveRequest(List<string> leaveRequestIds);
    }
}
