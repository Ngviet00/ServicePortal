using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.OverTime.Requests;
using ServicePortals.Application.Dtos.OverTime.Responses;
using ServicePortals.Application.Interfaces.OverTime;
using ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.OverTime
{
    [Route("api/overtime"), Authorize]
    [ApiController]
    public class OverTimeController : ControllerBase
    {
        private readonly IOverTimeService _overTimeService;

        public OverTimeController (IOverTimeService overTimeService)
        {
            _overTimeService = overTimeService;
        }

        [HttpGet("type-overtime")]
        public async Task<IActionResult> GetAllTypeOverTime()
        {
            var results = await _overTimeService.GetAllTypeOverTime();

            return Ok(new BaseResponse<List<TypeOverTime>>(200, "success", results));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateOverTimeRequest request)
        {
            var result = await _overTimeService.Create(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("get-my-overtime")]
        public async Task<IActionResult> GetMyOverTime([FromQuery] MyOverTimeRequest request)
        {
            var results = await _overTimeService.GetMyOverTime(request);

            var response = new PageResponse<MyOverTimeResponse>(
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

        [HttpGet("get-overtime-register")]
        public async Task<IActionResult> GetOverTimeRegister([FromQuery] MyOverTimeRequest request)
        {
            var results = await _overTimeService.GetOverTimeRegister(request);

            var response = new PageResponse<MyOverTimeRegisterResponse>(
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
            var result = await _overTimeService.Delete(applicationFormCode);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("{applicationFormCode}")]
        public async Task<object> GetDetailOverTimeByApplicationFormCode(string applicationFormCode)
        {
            var result = await _overTimeService.GetDetailOverTimeByApplicationFormCode(applicationFormCode);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPut("{applicationFormCode}")]
        public async Task<IActionResult> Update(string applicationFormCode, [FromForm] CreateOverTimeRequest request)
        {
            var result = await _overTimeService.Update(applicationFormCode, request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("reject-some-overtimes")]
        public async Task<IActionResult> RejectSomeOverTimes([FromBody] RejectSomeOverTimeRequest request)
        {
            var result = await _overTimeService.RejectSomeOverTimes(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("hr-note")]
        public async Task<IActionResult> HrNote([FromBody] HrNoteOverTimeRequest request)
        {
            var result = await _overTimeService.HrNote(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("hr-export-excel-overtime")]
        public async Task<IActionResult> HrExportExcelLeaveRequest([FromBody] long applicationFormId)
        {
            var fileContent = await _overTimeService.HrExportExcel(applicationFormId);

            var fileName = $"OverTime_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.xlsx";

            return File(fileContent ?? [], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("approval")]
        public async Task<IActionResult> Approval([FromBody] ApprovalRequest request)
        {
            var result = await _overTimeService.Approval(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }
    }
}
