using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Applications.Modules.MemoNotification.DTO;
using ServicePortal.Applications.Modules.MemoNotification.DTO.Requests;
using ServicePortal.Applications.Modules.MemoNotification.Services.Interfaces;
using ServicePortal.Common;
using ServicePortal.Common.Filters;

namespace ServicePortal.Applications.Modules.MemoNotification.Controllers
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

            var response = new PageResponse<MemoNotificationDto>(
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
        public async Task<IActionResult> GetAllInHomePage([FromQuery] int? DepartmentId)
        {
            var result = await _memoNotificationService.GetAllInHomePage(DepartmentId);

            return Ok(new BaseResponse<List<Domain.Entities.MemoNotification>>(200, "success", result));
        }


        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _memoNotificationService.GetById(id);

            return Ok(new BaseResponse<MemoNotificationDto>(200, "success", result));
        }

        [HttpPost("create"), RoleAuthorize(["HR", "union"])]
        public async Task<IActionResult> Create([FromForm] CreateMemoNotiDto request, [FromForm] IFormFile[] files)
        {
            var result = await _memoNotificationService.Create(request, files);

            return Ok(new BaseResponse<Domain.Entities.MemoNotification>(200, "success", result));
        }

        [HttpPut("update/{id}"), RoleAuthorize(["HR", "union"])]
        public async Task<IActionResult> Update(Guid id, [FromForm] CreateMemoNotiDto request, [FromForm] IFormFile[] files)
        {
            var result = await _memoNotificationService.Update(id, request, files);

            return Ok(new BaseResponse<Domain.Entities.MemoNotification>(200, "success", result));
        }

        [HttpDelete("delete/{id}"), RoleAuthorize(["HR", "union"])]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _memoNotificationService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.MemoNotification>(200, "success", result));
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var file = await _memoNotificationService.GetFileDownload(id);

            return File(file.FileData ?? [], file.ContentType ?? "application/octet-stream", file.FileName);
        }
    }
}
