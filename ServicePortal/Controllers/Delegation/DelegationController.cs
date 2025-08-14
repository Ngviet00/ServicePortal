using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.DelegatedTemp;
using ServicePortals.Application.Dtos.DelegatedTemp.Requests;
using ServicePortals.Application.Dtos.DelegatedTemp.Responses;
using ServicePortals.Application.Interfaces.DelegatedTemp;

namespace ServicePortal.Controllers.Delegation
{
    [ApiController, Route("api/delegation")]
    public class DelegationController : ControllerBase
    {
        //private readonly IDelegatedTempService _delegatedTempService;
        //public DelegationController(IDelegatedTempService delegatedTempService)
        //{
        //    _delegatedTempService = delegatedTempService;
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddNew([FromBody] CreateDelegatedTempRequest request)
        //{
        //    var result = await _delegatedTempService.AddNew(request);

        //    return Ok(new BaseResponse<DelegatedTempDto>(200, "success", result));
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetAll([FromQuery] DelegatedTempDto request)
        //{
        //    var result = await _delegatedTempService.GetAll(request);

        //    return Ok(new BaseResponse<List<GetAllDelegatedTempResponse>>(200, "success", result));
        //}

        //[HttpDelete]
        //public async Task<IActionResult> Delete([FromBody] DelegatedTempDto request)
        //{
        //    var result = await _delegatedTempService.Delete(request);

        //    return Ok(new BaseResponse<DelegatedTempDto>(200, "success", result));
        //}
    }
}
