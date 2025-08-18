using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application.Dtos.ITForm.Requests;
using ServicePortals.Application.Interfaces.ITForm;

namespace ServicePortal.Controllers.ITForm
{
    [Authorize]
    [ApiController, Route("api/it-form")]
    public class ITFormController : ControllerBase
    {
        private readonly ITFormService _iITFormService;

        public ITFormController(ITFormService iITFormService)
        {
            _iITFormService = iITFormService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(GetAllITFormRequest request)
        {
            var results = await _iITFormService.GetAll(request);

            return Ok(request);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _iITFormService.GetById(Id);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateITFormRequest request)
        {
            var result = await _iITFormService.Create(request);

            return Ok(result);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(Guid Id, [FromBody] UpdateITFormRequest request)
        {
            var result = await _iITFormService.Update(Id, request);

            return Ok(result);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _iITFormService.Delete(Id);

            return Ok(result);
        }
    }
}
