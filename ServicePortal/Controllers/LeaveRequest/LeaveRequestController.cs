using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.LeaveRequest;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.LeaveRequest.Responses;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Domain.Entities;

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
                results.TotalItems,
                results.CountPending,
                results.CountInProcess
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
                results.TotalItems,
                results.CountPending,
                results.CountInProcess
            );

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _leaveRequestService.Delete(id);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpDelete("delete-application-form-leave/{applicationFormId}")]
        public async Task<IActionResult> DeleteApplicationFormLeave(Guid applicationFormId)
        {
            var result = await _leaveRequestService.DeleteApplicationFormLeave(applicationFormId);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("get-list-leave-to-update/{id}")]
        public async Task<IActionResult> GetListLeaveToUpdate(Guid id)
        {
            var result = await _leaveRequestService.GetListLeaveToUpdate(id);

            return Ok(new BaseResponse<List<ServicePortals.Domain.Entities.LeaveRequest>>(200, "success", result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] CreateLeaveRequestDto request)
        {
            var result = await _leaveRequestService.Update(id, request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        //[HttpGet("get-my-leave-request-registered")]
        //public async Task<IActionResult> GetMyLeaveRequestRegisted()
        //{
        //    var results = await _leaveRequestService.GetMyLeaveRequest(request);

        //    var response = new PageResponse<MyLeaveRequestResponse>(
        //        200,
        //        "Success",
        //        results.Data,
        //        results.TotalPages,
        //        request.Page,
        //        request.PageSize,
        //        results.TotalItems,
        //        results.CountPending,
        //        results.CountInProcess
        //    );

        //    return Ok(response);
        //}

        //[HttpGet("statistical-leave-request")]
        //public async Task<IActionResult> StatisticalFormIT([FromQuery] int year)
        //{
        //    var results = await _leaveRequestService.StatisticalLeaveRequest(year);

        //    return Ok(new BaseResponse<object>(200, "success", results));
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetAll(GetAllLeaveRequest request)
        //{
        //    var results = await _leaveRequestService.GetAll(request);

        //    var response = new PageResponse<ServicePortals.Domain.Entities.LeaveRequest>(
        //        200,
        //        "Success",
        //        results.Data,
        //        results.TotalPages,
        //        request.Page,
        //        request.PageSize,
        //        results.TotalItems,
        //        results.CountPending,
        //        results.CountInProcess
        //    );

        //    return Ok(response);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _leaveRequestService.GetById(id);

            return Ok(new BaseResponse<ServicePortals.Domain.Entities.LeaveRequest>(200, "success", result));
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

        [HttpGet("search-user-register-leave-request")]
        public async Task<IActionResult> SearchUserRegisterLeaveRequest([FromQuery] SearchUserRegisterLeaveRequest request)
        {
            var result = await _leaveRequestService.SearchUserRegisterLeaveRequest(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        //[HttpPost("update-user-have-permission-hr-mng-leave-request")]
        //public async Task<IActionResult> UpdateUserHavePermissionHrMngLeaveRequest([FromBody] List<string> userCodes)
        //{
        //    var results = await _leaveRequestService.UpdateHrWithManagementLeavePermission(userCodes);

        //    return Ok(new BaseResponse<object>(200, "success", results));
        //}

        //[HttpGet("get-user-have-permission-hr-mng-leave-request")]
        //public async Task<IActionResult> GetUserHavePermissionHrMngLeaveRequest()
        //{
        //    var results = await _leaveRequestService.GetHrWithManagementLeavePermission();

        //    return Ok(new BaseResponse<List<HrMngLeaveRequestResponse>>(200, "success", results));
        //}

        //[HttpPost("hr-export-excel-leave-request")]
        //public async Task<IActionResult> HrExportExcelLeaveRequest([FromBody] List<string> leaveRequestIds)
        //{
        //    var fileContent = await _leaveRequestService.HrExportExcelLeaveRequest(leaveRequestIds);

        //    var fileName = $"LeaveRequests_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.xlsx";

        //    return File(fileContent ?? [], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //}
    }
}
