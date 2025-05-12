using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Common.Filters;
using ServicePortal.Modules.CustomApprovalFlow.DTO;
using ServicePortal.Modules.CustomApprovalFlow.Services.Interfaces;

namespace ServicePortal.Modules.CustomApprovalFlow.Controllers
{
    [Authorize]
    [ApiController, Route("api/custom-approval-flow"), RoleAuthorize("superadmin")]
    public class CustomApprovalFlowController : ControllerBase
    {
        private readonly ICustomApprovalFlowService _customApprovalFlowService;

        public CustomApprovalFlowController(ICustomApprovalFlowService customApprovalFlowService)
        {
            _customApprovalFlowService = customApprovalFlowService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery]CustomApprovalFlowDto request)
        {
            var results = await _customApprovalFlowService.GetAll(request);

            var response = new PageResponse<CustomApprovalFlowDto>(
                200,
                "Success",
                results.Data,
                results.TotalPages,
                results.TotalItems,
                request.Page,
                request.PageSize
            );

            return Ok(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _customApprovalFlowService.GetById(id);

            return Ok(new BaseResponse<CustomApprovalFlowDto>(200, "success", result));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CustomApprovalFlowDto dto)
        {
            var result = await _customApprovalFlowService.Create(dto);

            return Ok(new BaseResponse<Domain.Entities.CustomApprovalFlow>(200, "success", result));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomApprovalFlowDto dto)
        {
            var result = await _customApprovalFlowService.Update(id, dto);

            return Ok(new BaseResponse<Domain.Entities.CustomApprovalFlow>(200, "success", result));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customApprovalFlowService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.CustomApprovalFlow>(200, "success", result));
        }
    }
}
