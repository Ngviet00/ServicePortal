using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Common.Filters;
using ServicePortal.Modules.MemoNotification.DTO.Requests;
using ServicePortal.Modules.MemoNotification.Services.Interfaces;

namespace ServicePortal.Modules.MemoNotification.Controllers
{
    [ApiController, Route("api/memo-notification"), Authorize]
    public class MemoNotificationController : ControllerBase
    {
        private readonly IMemoNotificationService _memoNotificationService;

        public MemoNotificationController(IMemoNotificationService memoNotificationService)
        {
            _memoNotificationService = memoNotificationService;
        }

        [HttpGet("get-all"), RoleAuthorize(["HR", "union"])]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMemoNotiDto request)
        {
            var results = await _memoNotificationService.GetAll(request);

            var response = new PageResponse<Domain.Entities.MemoNotification>(
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

        [HttpGet("get-all-in-homepage")]
        public async Task<IActionResult> GetAllInHomePage()
        {
            var result = await _memoNotificationService.GetAllInHomePage();

            return Ok(new BaseResponse<List<Domain.Entities.MemoNotification>>(200, "success", result));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _memoNotificationService.GetById(id);

            return Ok(new BaseResponse<Domain.Entities.MemoNotification>(200, "success", result));
        }

        [HttpPost("create"), RoleAuthorize(["HR", "union"])]
        public async Task<IActionResult> Create([FromBody] CreateMemoNotiDto request)
        {
            var result = await _memoNotificationService.Create(request);

            return Ok(new BaseResponse<Domain.Entities.MemoNotification>(200, "success", result));
        }

        [HttpPut("update/{id}"), RoleAuthorize(["HR", "union"])]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateMemoNotiDto request)
        {
            var result = await _memoNotificationService.Update(id, request);

            return Ok(new BaseResponse<Domain.Entities.MemoNotification>(200, "success", result));
        }

        [HttpDelete("delete/{id}"), RoleAuthorize(["HR", "union"])]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _memoNotificationService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.MemoNotification>(200, "success", result));
        }
    }
}
