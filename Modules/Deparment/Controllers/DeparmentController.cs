using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Deparment.Interfaces;
using ServicePortal.Modules.Deparment.Requests;

namespace ServicePortal.Modules.Deparment.Controllers
{
    [Authorize]
    [ApiController, Route("api/deparment")]
    public class DeparmentController : ControllerBase
    {
        private readonly IDeparmentService _deparmentService;

        public DeparmentController(IDeparmentService deparmentService)
        {
            _deparmentService = deparmentService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(GetAllDeparmentRequest request)
        {
            var results = await _deparmentService.GetAll(request);

            var response = new PageResponse<Domain.Entities.Deparment>(200, "Success", results.Data, results.TotalPages, request.Page, request.PageSize, results.TotalItems);

            return Ok(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var deparment = await _deparmentService.GetById(id);

            return Ok(new BaseResponse<Domain.Entities.Deparment>(200, "success", deparment));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(DeparmentDTO dto)
        {
            var deparment = await _deparmentService.Create(dto);

            return Ok(new BaseResponse<Domain.Entities.Deparment>(200, "success", deparment));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, DeparmentDTO dto)
        {
            var deparment = await _deparmentService.Update(id, dto);

            return Ok(new BaseResponse<Domain.Entities.Deparment>(200, "success", deparment));
        }

        //[RoleAuthorize(RoleEnum.SuperAdmin)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deparment = await _deparmentService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.Deparment>(200, "success", deparment));
        }
    }
}
