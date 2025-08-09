using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application.Interfaces.Approval;
using ServicePortals.Application.Services.Approval;

namespace ServicePortal.Controllers.Approval
{
    [ApiController, Route("api/approval")]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;
        public ApprovalController(ApprovalService approvalService) 
        {
            _approvalService = approvalService;
        }

        [HttpPost("/approval")]
        public async Task<IActionResult> Approval()
        {
            return Ok();
        }

        [HttpGet("/list-wait-approval")]
        public async Task<IActionResult> ListWaitApproval()
        {
            return Ok();
        }

        [HttpGet("/list-assigned")]
        public async Task<IActionResult> ListAssigned()
        {
            return Ok();
        }

        [HttpGet("/list-history-approval-or-processed")]
        public async Task<IActionResult> ListHistoryApprovalOrProcessed()
        {
            return Ok();
        }
    }
}
