using Microsoft.AspNetCore.Mvc;
using ServicePortal.Filters;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.RequestType.Request;
using ServicePortals.Application.Interfaces.RequestType;
using Entities = ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.RequestType
{
    [ApiController, Route("api/request-type")]
    public class RequestTypeController : ControllerBase
    {
        private readonly IRequestTypeService _requestTypeService;

        public RequestTypeController(IRequestTypeService requestTypeService)
        {
            _requestTypeService = requestTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchRequestTypeRequest request)
        {
            var results = await _requestTypeService.GetAll(request);

            var response = new PageResponse<Entities.RequestType>(
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

        [HttpGet("{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _requestTypeService.GetById(id);

            return Ok(new BaseResponse<Entities.RequestType>(200, "success", role));
        }

        [HttpPost, RoleAuthorize("superadmin")]
        public async Task<IActionResult> Create([FromBody] CreateRequestTypeRequest request)
        {
            var role = await _requestTypeService.Create(request);

            return Ok(new BaseResponse<Entities.RequestType>(200, "success", role));
        }

        [HttpPut("{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateRequestTypeRequest request)
        {
            var role = await _requestTypeService.Update(id, request);

            return Ok(new BaseResponse<Entities.RequestType>(200, "success", role));
        }

        [HttpDelete("{id}"), RoleAuthorize("superadmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _requestTypeService.Delete(id);

            return Ok(new BaseResponse<Entities.RequestType>(200, "success", role));
        }
    }
}
