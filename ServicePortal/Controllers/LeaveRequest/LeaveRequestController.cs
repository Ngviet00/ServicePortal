using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.LeaveRequest;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.LeaveRequest.Responses;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Infrastructure.Excel;

namespace ServicePortal.Controllers.LeaveRequest
{
    [Authorize]
    [ApiController, Route("api/leave-request")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly ExcelService _excelService;

        public LeaveRequestController(
            ILeaveRequestService leaveRequestService,
            ExcelService excelService
        )
        {
            _excelService = excelService;
            _leaveRequestService = leaveRequestService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(GetAllLeaveRequest request)
        {
            var results = await _leaveRequestService.GetAll(request);

            var response = new PageResponse<LeaveRequestDto>(
                200,
                "Success",
                results.Data,
                results.TotalPages,
                request.Page,
                request.PageSize,
                results.TotalItems,
                results.CountPending,
                results.CountInProcess
            );

            return Ok(response);
        }

        [HttpGet("get-leave-request-wait-approval")]
        public async Task<IActionResult> GetWaitApproval(GetAllLeaveRequestWaitApprovalRequest request)
        {
            var userClaim = HttpContext.User;

            var results = await _leaveRequestService.GetAllWaitApproval(request, userClaim);

            var response = new PageResponse<LeaveRequestDto>
                (
                    200,
                    "Success",
                    results.Data,
                    results.TotalPages,
                    request.Page,
                    request.PageSize,
                    results.TotalItems
                );

            return Ok(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _leaveRequestService.GetById(id);

            return Ok(new BaseResponse<LeaveRequestDto>(200, "success", result));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequest request)
        {
            var userClaim = HttpContext.User;

            var result = await _leaveRequestService.Create(request, userClaim);

            return Ok(new BaseResponse<LeaveRequestDto>(200, "success", result));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, LeaveRequestDto dto)
        {
            var result = await _leaveRequestService.Update(id, dto);

            return Ok(new BaseResponse<LeaveRequestDto>(200, "success", result));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _leaveRequestService.Delete(id);

            return Ok(new BaseResponse<LeaveRequestDto>(200, "success", result));
        }

        [HttpPost("approval")]
        public async Task<IActionResult> Approval(ApprovalRequests request)
        {
            var userClaim = HttpContext.User;

            var currentUserCode = User.FindFirst("user_code")?.Value;

            var result = await _leaveRequestService.Approval(request, currentUserCode ?? "", userClaim);

            return Ok(new BaseResponse<LeaveRequestDto>(200, "success", result));
        }

        [HttpGet("count-wait-approval")]
        public async Task<IActionResult> CountWaitApproval(GetAllLeaveRequestWaitApprovalRequest request)
        {
            var userClaim = HttpContext.User;

            var result = await _leaveRequestService.CountWaitApproval(request, userClaim);

            return Ok(new BaseResponse<long>(200, "success", result));
        }

        [HttpPost("hr-register-all-leave-rq")]
        public async Task<IActionResult> HrRegisterAllLeave([FromBody] HrRegisterAllLeaveRequest request)
        {
            var result = await _leaveRequestService.HrRegisterAllLeave(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("history-approval")]
        public async Task<IActionResult> GetHistoryLeaveRequestApproval([FromQuery] GetAllLeaveRequest request)
        {
            var result = await _leaveRequestService.GetHistoryLeaveRequestApproval(request);

            var response = new PageResponse<LeaveRequestDto>(
                200,
                "Success",
                result.Data,
                result.TotalPages,
                request.Page,
                request.PageSize,
                result.TotalItems
            );

            return Ok(response);
        }

        [HttpPost("update-user-have-permission-create-multiple-leave-request")]
        public async Task<IActionResult> UpdateUserHavePermissionCreateMultipleLeaveRequest([FromBody] List<string> UserCodes)
        {
            var result = await _leaveRequestService.UpdateUserHavePermissionCreateMultipleLeaveRequest(UserCodes);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("get-usercode-have-permission-create-multiple-leave-request")]
        public async Task<IActionResult> GetUserCodeHavePermissionCreateMultipleLeaveRequest()
        {
            var result = await _leaveRequestService.GetUserCodeHavePermissionCreateMultipleLeaveRequest();

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("attach-user-manager-org-unit")]
        public async Task<IActionResult> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request)
        {
            var result = await _leaveRequestService.AttachUserManageOrgUnit(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("get-org-unit-id-attach-by-usercode")]
        public async Task<IActionResult> GetOrgUnitIdAttachedByUserCode([FromQuery] string userCode)
        {
            var result = await _leaveRequestService.GetOrgUnitIdAttachedByUserCode(userCode);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("create-leave-for-others")]
        public async Task<IActionResult> CreateLeaveForOther([FromBody] CreateLeaveRequestForManyPeopleRequest request)
        {
            var result = await _leaveRequestService.CreateLeaveForManyPeople(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("search-user-register-leave-request")]
        public async Task<IActionResult> SearchUserRegisterLeaveRequest([FromQuery] SearchUserRegisterLeaveRequest request)
        {
            var result = await _leaveRequestService.SearchUserRegisterLeaveRequest(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("export-leave-request-to-excel")]
        public async Task<IActionResult> ExportLeaveRequestToExcel()
        {
            //var data = new List<MyObject>
            //{
            //    new MyObject { Id = 1, Name = "Item 1", Date = DateTime.Now },
            //    new MyObject { Id = 2, Name = "Item 2", Date = DateTime.Now.AddDays(-1) }
            //};

            // Gọi hàm export
            var fileBytes = _excelService.ExportLeaveRequestToExcel();

            // Trả về file để download
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DataExport.xlsx");
        }

        [HttpPost("update-user-have-permission-hr-mng-leave-request")]
        public async Task<IActionResult> UpdateUserHavePermissionHrMngLeaveRequest([FromBody] List<string> userCodes)
        {
            var results = await _leaveRequestService.UpdateHrWithManagementLeavePermission(userCodes);

            return Ok(new BaseResponse<object>(200, "success", results));
        }

        [HttpGet("get-user-have-permission-hr-mng-leave-request")]
        public async Task<IActionResult> GetUserHavePermissionHrMngLeaveRequest()
        {
            var results = await _leaveRequestService.GetHrWithManagementLeavePermission();

            return Ok(new BaseResponse<List<HrMngLeaveRequestResponse>>(200, "success", results));
        }
    }
}
