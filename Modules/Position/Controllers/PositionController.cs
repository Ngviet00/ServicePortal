using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.Position.Interfaces;

namespace ServicePortal.Modules.Position.Controllers
{
    [ApiController, Route("position")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var positions = await _positionService.GetAll();

            return Ok(new BaseResponse<List<Domain.Entities.Position>>(200, "success", positions));
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var position = await _positionService.GetById(id);

            return Ok(new BaseResponse<Domain.Entities.Position>(200, "success", position));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(string name, int level)
        {
            var position = await _positionService.Create(name, level);

            return Ok(new BaseResponse<Domain.Entities.Position>(200, "success", position));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, string name, int level)
        {
            var position = await _positionService.Update(id, name, level);

            return Ok(new BaseResponse<Domain.Entities.Position>(200, "success", position));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var position = await _positionService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.Position>(200, "success", position));
        }

        [HttpDelete("force-delete/{id}")]
        public async Task<IActionResult> ForceDelete(int id)
        {
            var position = await _positionService.ForceDelete(id);

            return Ok(new BaseResponse<Domain.Entities.Position>(200, "success", position));
        }
    }
}
