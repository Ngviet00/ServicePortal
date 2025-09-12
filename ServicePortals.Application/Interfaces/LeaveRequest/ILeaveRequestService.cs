using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.LeaveRequest;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.LeaveRequest.Responses;

namespace ServicePortals.Application.Interfaces.LeaveRequest
{
    public interface ILeaveRequestService
    {
        Task<object> Create(CreateLeaveRequest request);
        Task<Domain.Entities.LeaveRequest> GetById(Guid Id);



        //Task<object> GetAll();
        //Task<object> Approval(ApprovalRequest request);
        //Task<object> Update(Guid Id, LeaveRequestDto dto);
        //Task<object> Delete(Guid Id);
        //Task<object> HrSaveNote(Guid Id);

        //Task<LeaveRequest> GetAllFormLeaveRequest();
        //Task<LeaveRequestStatisticalResponse> StatisticalLeaveRequest(int year);
        //Task<PagedResults<Domain.Entities.LeaveRequest>> GetAll(GetAllLeaveRequest request);
        

        //Task<object> UpdateUserHavePermissionCreateMultipleLeaveRequest(List<string> UserCodes);
        //Task<object> GetUserCodeHavePermissionCreateMultipleLeaveRequest();

        //Task<object> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request);
        //Task<object> GetOrgUnitIdAttachedByUserCode(string userCode);

        //Task<object> CreateLeaveForManyPeople(CreateLeaveRequestForManyPeopleRequest request);
        //Task<object> SearchUserRegisterLeaveRequest(SearchUserRegisterLeaveRequest request);
        //Task<object> HrRegisterAllLeave(HrRegisterAllLeaveRequest request);

        Task<List<HrMngLeaveRequestResponse>> GetHrWithManagementLeavePermission();
        //Task<object> UpdateHrWithManagementLeavePermission(List<string> UserCode);
        //Task<byte[]> HrExportExcelLeaveRequest(List<string> leaveRequestIds);
    }
}
