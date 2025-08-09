using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Filters;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.MemoNotification;
using ServicePortals.Application.Dtos.MemoNotification.Requests;
using ServicePortals.Application.Interfaces.MemoNotification;

namespace ServicePortal.Controllers.MemoNotification
{
    [ApiController, Route("api/memo-notification"), Authorize]
    public class MemoNotificationController : ControllerBase
    {
        private readonly IMemoNotificationService _memoNotificationService;

        public MemoNotificationController(IMemoNotificationService memoNotificationService)
        {
            _memoNotificationService = memoNotificationService;
        }

        [HttpGet("get-all")]
        [RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMemoNotiRequest request)
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

            return Ok(new BaseResponse<List<MemoNotificationDto>>(200, "success", result));
        }


        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _memoNotificationService.GetById(id);

            return Ok(new BaseResponse<MemoNotificationDto>(200, "success", result));
        }

        [HttpPost("create")]
        [RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        public async Task<IActionResult> Create([FromForm] CreateMemoNotiRequest request, [FromForm] IFormFile[] files)
        {
            var result = await _memoNotificationService.Create(request, files);

            return Ok(new BaseResponse<MemoNotificationDto>(200, "success", result));
        }

        [HttpPut("update/{id}")]
        [RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        public async Task<IActionResult> Update(Guid id, [FromForm] CreateMemoNotiRequest request, [FromForm] IFormFile[] files)
        {
            var result = await _memoNotificationService.Update(id, request, files);

            return Ok(new BaseResponse<MemoNotificationDto>(200, "success", result));
        }

        [HttpDelete("delete/{id}")]
        [RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _memoNotificationService.Delete(id);

            return Ok(new BaseResponse<MemoNotificationDto>(200, "success", result));
        }

        [HttpGet("download/{id}")]
        [RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var file = await _memoNotificationService.GetFileDownload(id);

            return File(file.FileData ?? [], file.ContentType ?? "application/octet-stream", file.FileName);
        }

        //[HttpGet("get-all-wait-approval")]
        //[RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        //public async Task<IActionResult> GetAllWaitApproval([FromQuery] MemoNotifyWaitApprovalRequest request)
        //{
        //    var results = await _memoNotificationService.GetWaitApproval(request);

        //    var response = new PageResponse<MemoNotificationDto>(
        //        200,
        //        "Success",
        //        results.Data,
        //        results.TotalPages,
        //        request.Page,
        //        request.PageSize,
        //        results.TotalItems
        //    );

        //    return Ok(response);
        //}

        //[HttpGet("get-all-history-approval")]
        //[RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        //public async Task<IActionResult> GetAllHistoryApproval([FromQuery] HistoryWaitApprovalMemoNotifyRequest request)
        //{
        //    var results = await _memoNotificationService.GetHistoryApproval(request);

        //    var response = new PageResponse<MemoNotificationDto>(
        //        200,
        //        "Success",
        //        results.Data,
        //        results.TotalPages,
        //        request.Page,
        //        request.PageSize,
        //        results.TotalItems
        //    );

        //    return Ok(response);
        //}

        //[HttpPost("approval")]
        //[RoleOrPermission("HR", "union", "IT", "memo_notification.create")]
        //public async Task<IActionResult> Approval([FromBody] ApprovalMemoNotifyRequest request)
        //{
        //    var result = await _memoNotificationService.Approval(request);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}
    }
}
