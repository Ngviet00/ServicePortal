using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.ITForm.Requests;
using ServicePortals.Application.Dtos.ITForm.Responses;
using ServicePortals.Application.Interfaces.ITForm;

namespace ServicePortal.Controllers.ITForm
{
    //[Authorize]
    [ApiController, Route("api/it-form")]
    public class ITFormController : ControllerBase
    {
        private readonly ITFormService _iITFormService;

        public ITFormController(ITFormService iITFormService)
        {
            _iITFormService = iITFormService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllITFormRequest request)
        {
            var results = await _iITFormService.GetAll(request);

            var response = new PageResponse<ITFormResponse>(
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

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _iITFormService.GetById(Id);

            return Ok(new BaseResponse<ITFormResponse>(200, "success", result));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateITFormRequest request)
        {
            var result = await _iITFormService.Create(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(Guid Id, [FromBody] UpdateITFormRequest request)
        {
            var result = await _iITFormService.Update(Id, request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _iITFormService.Delete(Id);

            return Ok(new BaseResponse<object>(200, "success", result));
        }
    }
}
