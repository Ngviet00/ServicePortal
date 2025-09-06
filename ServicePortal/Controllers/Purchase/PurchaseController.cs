using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.Purchase.Requests;
using ServicePortals.Application.Interfaces.Purchase;
using ServicePortals.Shared.SharedDto;
using ServicePortals.Shared.SharedDto.Requests;

namespace ServicePortal.Controllers.Purchase
{
    [ApiController, Route("api/purchase")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        public PurchaseController (IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet("statistical-purchase")]
        public async Task<IActionResult> StatisticalFormIT([FromQuery] int year)
        {
            var results = await _purchaseService.StatisticalPurchase(year);

            return Ok(new BaseResponse<object>(200, "success", results));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPurchaseRequest request)
        {
            var results = await _purchaseService.GetAll(request);

            var response = new PageResponse<ServicePortals.Domain.Entities.Purchase>(
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _purchaseService.GetById(id);

            return Ok(new BaseResponse<ServicePortals.Domain.Entities.Purchase>(200, "success", result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseRequest request)
        {
            var result = await _purchaseService.Create(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePurchaseRequest request)
        {
            var result = await _purchaseService.Update(id, request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _purchaseService.Delete(id);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("assigned-task")]
        public async Task<IActionResult> AssignedTask([FromBody] AssignedTaskRequest request)
        {
            var result = await _purchaseService.AssignedTask(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPost("resolved-task")]
        public async Task<IActionResult> ResolvedTask([FromBody] ResolvedTaskRequest request)
        {
            var result = await _purchaseService.ResolvedTask(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpGet("get-member-purchase-assigned")]
        public async Task<IActionResult> GetMemberPurchaseAssigned()
        {
            var results = await _purchaseService.GetMemberPurchaseAssigned();

            return Ok(new BaseResponse<List<InfoUserAssigned>>(200, "success", results));
        }
    }
}
