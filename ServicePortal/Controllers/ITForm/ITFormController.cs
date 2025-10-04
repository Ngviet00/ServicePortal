using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.ITForm.Requests;
using ServicePortals.Application.Dtos.ITForm.Responses;
using ServicePortals.Application.Interfaces.ITForm;
using ServicePortals.Shared.SharedDto;
using ServicePortals.Shared.SharedDto.Requests;
using Entity = ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.ITForm
{
    [Authorize]
    [ApiController, Route("api/it-form")]
    public class ITFormController : ControllerBase
    {
        //private readonly ITFormService _iITFormService;

        //public ITFormController(ITFormService iITFormService)
        //{
        //    _iITFormService = iITFormService;
        //}

        //[HttpGet("statistical-form-it")]
        //public async Task<IActionResult> StatisticalFormIT([FromQuery] int year)
        //{
        //    var results = await _iITFormService.StatisticalFormIT(year);

        //    return Ok(new BaseResponse<object>(200, "success", results));
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetAll([FromQuery] GetAllITFormRequest request)
        //{
        //    var results = await _iITFormService.GetAll(request);

        //    var response = new PageResponse<GetListITFormResponse>(
        //        200,
        //        "Success",
        //        results.Data,
        //        results.TotalPages,
        //        request.Page,
        //        request.PageSize,
        //        results.TotalItems
        //    );

        //    return Ok(response);
        //}

        //[HttpGet("{Id}")]
        //public async Task<IActionResult> GetById(Guid Id)
        //{
        //    var result = await _iITFormService.GetById(Id);

        //    return Ok(new BaseResponse<Entity.ITForm>(200, "success", result));
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateITFormRequest request)
        //{
        //    var result = await _iITFormService.Create(request);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}

        //[HttpPut("{Id}")]
        //public async Task<IActionResult> Update(Guid Id, [FromBody] UpdateITFormRequest request)
        //{
        //    var result = await _iITFormService.Update(Id, request);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}

        //[HttpDelete("{Id}")]
        //public async Task<IActionResult> Delete(Guid Id)
        //{
        //    var result = await _iITFormService.Delete(Id);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}

        //[HttpPost("assigned-task")]
        //public async Task<IActionResult> AssignedTask([FromBody] AssignedTaskRequest request)
        //{
        //    var result = await _iITFormService.AssignedTask(request);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}

        //[HttpPost("resolved-task")]
        //public async Task<IActionResult> ResolvedTask([FromBody] ResolvedTaskRequest request)
        //{
        //    var result = await _iITFormService.ResolvedTask(request);

        //    return Ok(new BaseResponse<object>(200, "success", result));
        //}


        //[HttpGet("get-member-it-assigned")]
        //public async Task<IActionResult> GetMemberITAssigned()
        //{
        //    var results = await _iITFormService.GetMemberITAssigned();

        //    return Ok(new BaseResponse<List<InfoUserAssigned>>(200, "success", results));
        //}
    }
}
