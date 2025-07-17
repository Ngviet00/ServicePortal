using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.SystemConfig;
using ServicePortals.Application.Interfaces.SystemConfig;

namespace ServicePortal.Controllers.SystemConfig
{
    //[Authorize]
    [ApiController, Route("api/system-config")]
    public class SystemConfigController : ControllerBase
    {
        private readonly ISystemConfigService _systemConfigService;
        public SystemConfigController(ISystemConfigService systemConfigService)
        {
            _systemConfigService = systemConfigService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var results = await _systemConfigService.GetAll();

            return Ok(new BaseResponse<List<SystemConfigDto?>>(200, "success", results));
        }

        [HttpGet("get-config-by-key")]
        public async Task<IActionResult> GetByConfigKey(string configkey)
        {
            var results = await _systemConfigService.GetByConfigKey(configkey);

            return Ok(new BaseResponse<SystemConfigDto?>(200, "success", results));
        }

        [HttpPost("add-config")]
        public async Task<IActionResult> AddConfig(SystemConfigDto request)
        {
            var results = await _systemConfigService.AddConfig(request);

            return Ok(new BaseResponse<SystemConfigDto?>(200, "success", results));
        }

        [HttpPut("update-config/{configkey}")]
        public async Task<IActionResult> UpdateConfig(string configkey, SystemConfigDto request)
        {
            var results = await _systemConfigService.UpdateConfig(configkey, request);

            return Ok(new BaseResponse<SystemConfigDto?>(200, "success", results));
        }
    }
}
