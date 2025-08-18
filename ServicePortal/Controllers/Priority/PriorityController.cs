using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.Priority.Requests;
using ServicePortals.Application.Interfaces.Priority;
using Entity = ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.Priority
{
    [Authorize]
    [ApiController, Route("api/priority")]
    public class PriorityController : ControllerBase
    {
        private readonly IPriorityService _priorityService;

        public PriorityController(IPriorityService priorityService)
        {
            _priorityService = priorityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(GetAllPriorityRequest request)
        {
            var results = await _priorityService.GetAll(request);

            return Ok(new BaseResponse<List<Entity.Priority>>(200, "success", results));
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var result = await _priorityService.GetById(Id);

            return Ok(new BaseResponse<Entity.Priority>(200, "success", result));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePriorityRequest request)
        {
            var result = await _priorityService.Create(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] UpdatePriorityRequest request)
        {
            var result = await _priorityService.Update(Id, request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var result = await _priorityService.Delete(Id);

            return Ok(new BaseResponse<object>(200, "success", result));
        }
    }
}
