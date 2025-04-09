using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.Deparment.Interfaces;

namespace ServicePortal.Modules.Deparment.Controllers
{
    public class DeparmentController : ControllerBase
    {
        private readonly IDeparmentService _deparmentService;

        public DeparmentController (IDeparmentService deparmentService)
        {
            _deparmentService = deparmentService;
        }

        [HttpGet("/get-all")]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _deparmentService.GetAll();

            return Ok(new BaseResponse<List<Domain.Entities.Deparment>>(200, "success", roles));
        }

        [HttpGet("/get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _deparmentService.GetById(id);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }

        [HttpPost("/create")]
        public async Task<IActionResult> Create(string name)
        {
            var role = await _deparmentService.Create(name);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }

        [HttpPut("/update/{id}")]
        public async Task<IActionResult> Update(int id, string name)
        {
            var role = await _deparmentService.Update(id, name);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }

        [HttpDelete("/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _deparmentService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }

        [HttpDelete("/force-delete/{id}")]
        public async Task<IActionResult> ForceDelete(int id)
        {
            var role = await _deparmentService.Delete(id);

            return Ok(new BaseResponse<Domain.Entities.Role>(200, "success", role));
        }
    }
}
