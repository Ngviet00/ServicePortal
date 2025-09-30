using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application;
using ServicePortals.Application.Interfaces.InternalMemoHR;
using ServicePortals.Domain.Entities;
using ServicePortals.Application.Dtos.InternalMemoHR.Requests;

namespace ServicePortal.Controllers.InternalMemoHR
{
    [Route("api/internal-memo-hr"), Authorize]
    [ApiController]
    public class InternalMemoHrController : ControllerBase
    {
        private readonly InternalMemoHrService _internalMemoHrService;

        public InternalMemoHrController(InternalMemoHrService internalMemoHrService)
        {
            _internalMemoHrService = internalMemoHrService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] GetListInternalMemoHrRequest request)
        {
            var results = await _internalMemoHrService.GetList(request);

            var response = new PageResponse<ApplicationForm>(
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

        [HttpGet("{applicationFormCode}")]
        public async Task<object> GetDetailInternalMemoByApplicationFormCode(string applicationFormCode)
        {
            var result = await _internalMemoHrService.GetDetailInternalMemoByApplicationFormCode(applicationFormCode);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInternalMemoHrRequest request)
        {
            var result = await _internalMemoHrService.Create(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpDelete("{applicationFormCode}")]
        public async Task<IActionResult> Delete(string applicationFormCode)
        {
            var result = await _internalMemoHrService.Delete(applicationFormCode);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPut("{applicationFormCode}")]
        public async Task<IActionResult> Update(string applicationFormCode, [FromBody] CreateInternalMemoHrRequest request)
        {
            var result = await _internalMemoHrService.Update(applicationFormCode, request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("approval")]
        public async Task<IActionResult> Approval([FromBody] ApprovalRequest request)
        {
            var result = await _internalMemoHrService.Approval(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }
    }
}
