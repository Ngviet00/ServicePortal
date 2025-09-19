using Microsoft.AspNetCore.Http;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.LeaveRequest;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.LeaveRequest.Responses;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.LeaveRequest
{
    public interface ILeaveRequestService
    {
        Task<object> Create(CreateLeaveRequest request);
        Task<PagedResults<MyLeaveRequestResponse>> GetMyLeaveRequest(MyLeaveRequest request);
        Task<PagedResults<MyLeaveRequestRegisteredResponse>> GetMyLeaveRequestRegistered(MyLeaveRequestRegistered request);
        Task<object> DeleteApplicationFormLeave(Guid ApplicationFormId);
        Task<List<Domain.Entities.LeaveRequest>> GetListLeaveToUpdate(Guid Id);
        Task<Domain.Entities.LeaveRequest> GetById(Guid Id);
        Task<object> Update(Guid Id, List<CreateLeaveRequestDto> dto);
        Task<ViewDetailLeaveRequestWithHistoryResponse?> ViewDetailLeaveRequestWithHistory(Guid Id);
        Task<object> SearchUserRegisterLeaveRequest(SearchUserRegisterLeaveRequest request);


        //Task<object> Approval(ApprovalRequest request);
        //Task<object> HrSaveNote(Guid Id);

        //Task<LeaveRequest> GetAllFormLeaveRequest();
        //Task<LeaveRequestStatisticalResponse> StatisticalLeaveRequest(int year);


        //Task<object> UpdateUserHavePermissionCreateMultipleLeaveRequest(List<string> UserCodes);
        //Task<object> GetUserCodeHavePermissionCreateMultipleLeaveRequest();

        //Task<object> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request);
        //Task<object> GetOrgUnitIdAttachedByUserCode(string userCode);

        //Task<object> CreateLeaveForManyPeople(CreateLeaveRequestForManyPeopleRequest request);
        //Task<object> HrRegisterAllLeave(HrRegisterAllLeaveRequest request);

        //Task<List<HrMngLeaveRequestResponse>> GetHrWithManagementLeavePermission();
        //Task<object> UpdateHrWithManagementLeavePermission(List<string> UserCode);
        //Task<byte[]> HrExportExcelLeaveRequest(List<string> leaveRequestIds);
    }
}
