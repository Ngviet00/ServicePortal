using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.LeaveRequest.Responses;
using ServicePortals.Application.Interfaces.LeaveRequest;

namespace ServicePortal.Controllers.LeaveRequest
{
    [Authorize]
    [ApiController, Route("api/leave-request")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(
            ILeaveRequestService leaveRequestService
        )
        {
            _leaveRequestService = leaveRequestService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateLeaveRequest request)
        {
            var result = await _leaveRequestService.Create(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("get-my-leave-request-application")]
        public async Task<IActionResult> GetMyLeaveRequest([FromQuery] MyLeaveRequest request)
        {
            var results = await _leaveRequestService.GetMyLeaveRequest(request);

            var response = new PageResponse<MyLeaveRequestResponse>(
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

        [HttpGet("get-my-leave-request-registered")]
        public async Task<IActionResult> GetMyLeaveRequestRegistered([FromQuery] MyLeaveRequestRegistered request)
        {
            var results = await _leaveRequestService.GetMyLeaveRequestRegistered(request);

            var response = new PageResponse<MyLeaveRequestRegisteredResponse>(
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

        [HttpDelete("{applicationFormCode}")]
        public async Task<IActionResult> Delete(string applicationFormCode)
        {
            var result = await _leaveRequestService.Delete(applicationFormCode);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPut("{applicationFormCode}")]
        public async Task<IActionResult> Update(string applicationFormCode, [FromBody] List<CreateListLeaveRequest> request)
        {
            var result = await _leaveRequestService.Update(applicationFormCode, request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("{applicationFormCode}")]
        public async Task<IActionResult> GetLeaveByAppliationFormCode(string applicationFormCode)
        {
            var result = await _leaveRequestService.GetLeaveByAppliationFormCode(applicationFormCode);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("search-user-register-leave-request")]
        public async Task<IActionResult> SearchUserRegisterLeaveRequest([FromQuery] SearchUserRegisterLeaveRequest request)
        {
            var result = await _leaveRequestService.SearchUserRegisterLeaveRequest(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("reject-some-leaves")]
        public async Task<IActionResult> RejectSomeLeaves([FromBody] RejectSomeLeavesRequest request)
        {
            var result = await _leaveRequestService.RejectSomeLeaves(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("hr-export-excel-leave-request")]
        public async Task<IActionResult> HrExportExcelLeaveRequest([FromBody] long applicationFormId)
        {
            var fileContent = await _leaveRequestService.HrExportExcelLeaveRequest(applicationFormId);

            var fileName = $"LeaveRequests_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.xlsx";

            return File(fileContent ?? [], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("hr-note")]
        public async Task<IActionResult> HrNote([FromBody] HrNoteRequest request)
        {
            var result = await _leaveRequestService.HrNote(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("approval")]
        public async Task<IActionResult> Approval([FromBody] ApprovalRequest request)
        {
            var result = await _leaveRequestService.Approval(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        //[HttpPost("hr-register-all-leave-rq")]
        //public async Task<IActionResult> HrRegisterAllLeave([FromBody] HrRegisterAllLeaveRequest request)
        //{
        //    var result = await _leaveRequestService.HrRegisterAllLeave(request);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}

        //[HttpPost("update-user-have-permission-create-multiple-leave-request")]
        //public async Task<IActionResult> UpdateUserHavePermissionCreateMultipleLeaveRequest([FromBody] List<string> UserCodes)
        //{
        //    var result = await _leaveRequestService.UpdateUserHavePermissionCreateMultipleLeaveRequest(UserCodes);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}

        //[HttpGet("get-usercode-have-permission-create-multiple-leave-request")]
        //public async Task<IActionResult> GetUserCodeHavePermissionCreateMultipleLeaveRequest()
        //{
        //    var result = await _leaveRequestService.GetUserCodeHavePermissionCreateMultipleLeaveRequest();

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}

        //[HttpPost("attach-user-manager-org-unit")]
        //public async Task<IActionResult> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request)
        //{
        //    var result = await _leaveRequestService.AttachUserManageOrgUnit(request);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}

        //[HttpGet("get-org-unit-id-attach-by-usercode")]
        //public async Task<IActionResult> GetOrgUnitIdAttachedByUserCode([FromQuery] string userCode)
        //{
        //    var result = await _leaveRequestService.GetOrgUnitIdAttachedByUserCode(userCode);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}

        //[HttpPost("create-leave-for-others")]
        //public async Task<IActionResult> CreateLeaveForOther([FromBody] CreateLeaveRequestForManyPeopleRequest request)
        //{
        //    var result = await _leaveRequestService.CreateLeaveForManyPeople(request);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}



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
