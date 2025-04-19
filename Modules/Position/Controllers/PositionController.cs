using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.Position.DTO;
using ServicePortal.Modules.Position.Interfaces;
using ServicePortal.Modules.Position.Requests;

namespace ServicePortal.Modules.Position.Controllers
{
    [ApiController, Route("api/position")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(GetAllPositionRequest request)
        {
            var results = await _positionService.GetAll(request);

            var response = new PageResponse<PositionDTO>(200, "Success", results.Data, results.TotalPages, request.Page, request.PageSize, results.TotalItems);

            return Ok(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _positionService.GetById(id);

            return Ok(new BaseResponse<PositionDTO>(200, "success", result));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(PositionDTO dto)
        {
            var result = await _positionService.Create(dto);

            return Ok(new BaseResponse<PositionDTO>(200, "success", result));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, PositionDTO dto)
        {
            var result = await _positionService.Update(id, dto);

            return Ok(new BaseResponse<PositionDTO>(200, "success", result));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _positionService.Delete(id);

            return Ok(new BaseResponse<PositionDTO>(200, "success", null));
        }

        [HttpDelete("force-delete/{id}")]
        public async Task<IActionResult> ForceDelete(int id)
        {
            var result = await _positionService.ForceDelete(id);

            return Ok(new BaseResponse<PositionDTO>(200, "success", null));
        }
    }
}
