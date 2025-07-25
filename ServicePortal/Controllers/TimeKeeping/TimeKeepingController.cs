﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.TimeKeeping.Requests;
using ServicePortals.Application.Dtos.TimeKeeping.Responses;
using ServicePortals.Application.Interfaces.TimeKeeping;

namespace ServicePortal.Controllers.TimeKeeping
{
    [Authorize]
    [ApiController, Route("api/time-keeping")]
    public class TimeKeepingController : ControllerBase
    {
        private readonly ITimeKeepingService _timeKeepingService;

        public TimeKeepingController(ITimeKeepingService timeKeepingService)
        {
            _timeKeepingService = timeKeepingService;
        }

        [HttpGet("get-personal-time-keeping")]
        public async Task<IActionResult> GetPersonalTimeKeeping([FromQuery] GetPersonalTimeKeepingRequest request)
        {
            var results = await _timeKeepingService.GetPersonalTimeKeeping(request);
            return Ok(new BaseResponse<object>(200, "Success", results));
        }

        [HttpGet("get-management-time-keeping")]
        public async Task<IActionResult> GetMngTimeKeeping([FromQuery] GetManagementTimeKeepingRequest request)
        {
            var results = await _timeKeepingService.GetManagementTimeKeeping(request);

            var response = new PageResponse<GroupedUserTimeKeeping>(
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

        [HttpPost("confirm-timekeeping-to-hr")]
        public async Task<IActionResult> ConfirmTimeKeepingToHr([FromBody] GetManagementTimeKeepingRequest request)
        {
            var results = await _timeKeepingService.ConfirmTimeKeepingToHr(request);
            return Ok(new BaseResponse<object>(200, "Success", results));
        }

        [HttpPost("update-user-have-permission-mng-timekeeping")]
        public async Task<IActionResult> UpdateUserHavePermissionMngTimeKeeping([FromBody] List<string> userCodes)
        {
            var results = await _timeKeepingService.UpdateUserHavePermissionMngTimeKeeping(userCodes);

            return Ok(new BaseResponse<object>(200, "success", results));
        }

        [HttpGet("get-user-have-permission-mng-timekeeping")]
        public async Task<IActionResult> GetUserHavePermissionMngTimeKeeping()
        {
            var results = await _timeKeepingService.GetUserHavePermissionMngTimeKeeping();

            return Ok(new BaseResponse<object>(200, "success", results));
        }

        [HttpPost("attach-user-manager-org-unit")]
        public async Task<IActionResult> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request)
        {
            var result = await _timeKeepingService.AttachUserManageOrgUnit(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("get-org-unit-id-attach-by-usercode")]
        public async Task<IActionResult> GetOrgUnitIdAttachedByUserCode([FromQuery] string userCode)
        {
            var result = await _timeKeepingService.GetOrgUnitIdAttachedByUserCode(userCode);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("change-user-mng-timekeeping")]
        public async Task<IActionResult> ChangeUserMngTimeKeeping(ChangeUserMngTimeKeepingRequest request)
        {
            var results = await _timeKeepingService.ChangeUserMngTimeKeeping(request);

            return Ok(new BaseResponse<object>(200, "success", results));
        }
    }
}
