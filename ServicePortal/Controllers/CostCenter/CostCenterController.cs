using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Interfaces.CostCenter;

namespace ServicePortal.Controllers.CostCenter
{
    [ApiController, Route("api/cost-center")]
    public class CostCenterController : ControllerBase
    {
        private readonly ICostCenterService _costCenterService;

        public CostCenterController (ICostCenterService costCenterService)
        {
            _costCenterService = costCenterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _costCenterService.GetAll();

            return Ok(new BaseResponse<List< ServicePortals.Domain.Entities.CostCenter>>(200, "success", results));
        }
    }
}
