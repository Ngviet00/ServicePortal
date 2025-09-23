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
        /// <summary>
        /// Hàm tạo đơn nghỉ phép, đăng ký thủ công 1 người, nhiều người, đăng ký bằng excel
        /// </summary>
        Task<object> Create(CreateLeaveRequest request); 

        /// <summary>
        /// Danh sách đơn xin nghỉ phép của cá nhân
        /// </summary>
        Task<PagedResults<MyLeaveRequestResponse>> GetMyLeaveRequest(MyLeaveRequest request);

        /// <summary>
        /// Danh sách các đơn nghỉ phép đã đăng ký, đã đăng ký cho chính mình, người khác
        /// </summary>
        Task<PagedResults<MyLeaveRequestRegisteredResponse>> GetMyLeaveRequestRegistered(MyLeaveRequestRegistered request);

        /// <summary>
        /// Xóa đơn nghỉ phép, set deleted at, set delete các bảng liên quan application form, application form item
        /// </summary>
        Task<object> Delete(string ApplicationFormCode);

        //Task<List<Domain.Entities.LeaveRequest>> GetListLeaveToUpdate(Guid Id);

        /// <summary>
        /// Lấy chi tiết đơn nghỉ phép, bao gồm trạng thái, lịch sử phê duyệt
        /// </summary>
        Task<object> GetLeaveByAppliationFormCode(string applicationFormCode);

        //update
        Task<object> Update(string applicationFormCode, List<CreateListLeaveRequest> request);

        /// <summary>
        /// Tìm kiếm người dùng khi nhập mã nhân viên ở màn hình tạo đơn nghỉ phép
        /// </summary>
        Task<object> SearchUserRegisterLeaveRequest(SearchUserRegisterLeaveRequest request);

        /// <summary>
        /// Từ chối 1 vài đơn của người khác
        /// </summary>
        Task<object> RejectSomeLeaves(RejectSomeLeavesRequest request);
        Task<object> Approval(ApprovalRequest request);
        Task<byte[]> HrExportExcelLeaveRequest(long applicationFormId);
        Task<object> HrNote(HrNoteRequest request);


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
    }
}
