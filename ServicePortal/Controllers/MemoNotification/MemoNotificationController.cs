using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Filters;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.MemoNotification.Requests;
using ServicePortals.Application.Dtos.MemoNotification.Responses;
using ServicePortals.Application.Interfaces.MemoNotification;
using Entities = ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.MemoNotification
{
    [Authorize]
    [ApiController, Route("api/memo-notification")]
    public class MemoNotificationController : ControllerBase
    {
        private readonly IMemoNotificationService _memoNotificationService;

        public MemoNotificationController(IMemoNotificationService memoNotificationService)
        {
            _memoNotificationService = memoNotificationService;
        }

        [HttpGet]
        [RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMemoNotificationRequest request)
        {
            var results = await _memoNotificationService.GetAll(request);

            var response = new PageResponse<GetAllMemoNotifyResponse>(
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

            return Ok(new BaseResponse<List<GetAllMemoNotifyResponse>>(200, "success", result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _memoNotificationService.GetById(id);

            return Ok(new BaseResponse<Entities.MemoNotification>(200, "success", result));
        }

        [HttpPost]
        [RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        public async Task<IActionResult> Create([FromForm] CreateMemoNotificationRequest request, [FromForm] IFormFile[] files)
        {
            var result = await _memoNotificationService.Create(request, files);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPut("{id}")]
        [RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        public async Task<IActionResult> Update(Guid id, [FromForm] CreateMemoNotificationRequest request, [FromForm] IFormFile[] files)
        {
            var result = await _memoNotificationService.Update(id, request, files);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpDelete("{id}")]
        [RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _memoNotificationService.Delete(id);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var file = await _memoNotificationService.GetFileDownload(id);

            return File(file.FileData ?? [], file.ContentType ?? "application/octet-stream", file.FileName);
        }
    }
}
