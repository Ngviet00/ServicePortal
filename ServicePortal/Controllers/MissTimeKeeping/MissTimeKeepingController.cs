using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application;
using ServicePortals.Application.Interfaces.MissTimeKeeping;
using ServicePortals.Application.Dtos.MissTimeKeeping.Requests;
using ServicePortals.Application.Dtos.MissTimeKeeping.Responses;

namespace ServicePortal.Controllers.MissTimeKeeping;

[Route("api/miss-timekeeping"), Authorize]
[ApiController]
public class MissTimeKeepingController : ControllerBase
{
    private readonly IMissTimeKeepingService _missTimeKeepingService;

    public MissTimeKeepingController(IMissTimeKeepingService missTimeKeepingService)
    {
        _missTimeKeepingService = missTimeKeepingService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMissTimeKeepingRequest request)
    {
        var result = await _missTimeKeepingService.Create(request);

        return Ok(new BaseResponse<object>(200, "success", result));
    }

    [HttpGet("get-my-miss-timekeeping")]
    public async Task<IActionResult> GetMyMissTimeKeeping([FromQuery] MyMissTimeKeepingRequest request)
    {
        var results = await _missTimeKeepingService.GetMyMissTimeKeeping(request);

        var response = new PageResponse<MyMissTimeKeepingResponse>(
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

    [HttpGet("get-miss-timekeeping-register")]
    public async Task<IActionResult> GetMissTimeKeepingRegister([FromQuery] MyMissTimeKeepingRequest request)
    {
        var results = await _missTimeKeepingService.GetMissTimeKeepingRegister(request);

        var response = new PageResponse<MyMissTimeKeepingRegisterResponse>(
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
        var result = await _missTimeKeepingService.Delete(applicationFormCode);

        return Ok(new BaseResponse<object>(200, "success", result));
    }

    [HttpGet("{applicationFormCode}")]
    public async Task<object> GetMissTimeKeepingByApplicationFormCode(string applicationFormCode)
    {
        var result = await _missTimeKeepingService.GetMissTimeKeepingByApplicationFormCode(applicationFormCode);

        return Ok(new BaseResponse<object>(200, "success", result));
    }

    [HttpPut("{applicationFormCode}")]
    public async Task<IActionResult> Update(string applicationFormCode, [FromBody] CreateMissTimeKeepingRequest request)
    {
        var result = await _missTimeKeepingService.Update(applicationFormCode, request);

        return Ok(new BaseResponse<object>(200, "success", result));
    }

    [HttpPost("hr-note")]
    public async Task<IActionResult> HrNote([FromBody] HrNoteMissTimeKeepingRequest request)
    {
        var result = await _missTimeKeepingService.HrNote(request);

        return Ok(new BaseResponse<object>(200, "success", result));
    }

    [HttpPost("hr-export-excel-miss-timekeeping")]
    public async Task<IActionResult> HrExportExcelMissTimeKeeping([FromBody] long applicationFormId)
    {
        var fileContent = await _missTimeKeepingService.HrExportExcel(applicationFormId);

        var fileName = $"Miss_TimeKeeping_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.xlsx";

        return File(fileContent ?? [], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost("approval")]
    public async Task<IActionResult> Approval([FromBody] ApprovalRequest request)
    {
        var result = await _missTimeKeepingService.Approval(request);

        return Ok(new BaseResponse<object>(200, "success", result));
    }
}