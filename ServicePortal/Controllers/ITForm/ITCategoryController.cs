using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.ITForm.Requests.ITCategory;
using ServicePortals.Application.Interfaces.ITForm;
using Entity = ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.ITForm
{
    [Authorize]
    [ApiController, Route("api/it-category")]
    public class ITCategoryController : ControllerBase
    {
        private readonly ITCategoryService _iTCategoryService;

        public ITCategoryController(ITCategoryService iTCategoryService)
        {
            _iTCategoryService = iTCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _iTCategoryService.GetAll();

            return Ok(new BaseResponse<List<Entity.ITCategory>>(200, "success", results));
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var result = await _iTCategoryService.GetById(Id);

            return Ok(new BaseResponse<Entity.ITCategory>(200, "success", result));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateITCategoryRequest request)
        {
            var result = await _iTCategoryService.Create(request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] UpdateITCategoryRequest request)
        {
            var result = await _iTCategoryService.Update(Id, request);

            return Ok(new BaseResponse<object>(200, "success", result));
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var result = await _iTCategoryService.Delete(Id);

            return Ok(new BaseResponse<object>(200, "success", result));
        }
    }
}
