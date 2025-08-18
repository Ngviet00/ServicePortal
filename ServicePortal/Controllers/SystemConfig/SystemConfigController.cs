using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePortals.Application;
using ServicePortals.Application.Interfaces.SystemConfig;
using Entities = ServicePortals.Domain.Entities;

namespace ServicePortal.Controllers.SystemConfig
{
    [Authorize]
    [ApiController, Route("api/system-config")]
    public class SystemConfigController : ControllerBase
    {
        private readonly ISystemConfigService _systemConfigService;
        public SystemConfigController(ISystemConfigService systemConfigService)
        {
            _systemConfigService = systemConfigService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _systemConfigService.GetAll();

            return Ok(new BaseResponse<List<Entities.SystemConfig>>(200, "success", results));
        }

        [HttpGet("{configKey}")]
        public async Task<IActionResult> GetByConfigKey(string configkey)
        {
            var result = await _systemConfigService.GetByConfigKey(configkey);

            return Ok(new BaseResponse<Entities.SystemConfig>(200, "success", result));
        }

        [HttpPost]
        public async Task<IActionResult> AddConfig(Entities.SystemConfig request)
        {
            var result = await _systemConfigService.AddConfig(request);

            return Ok(new BaseResponse<Entities.SystemConfig>(200, "success", result));
        }

        [HttpPut("{configkey}")]
        public async Task<IActionResult> UpdateConfig(string configkey, Entities.SystemConfig request)
        {
            var results = await _systemConfigService.UpdateConfig(configkey, request);

            return Ok(new BaseResponse<Entities.SystemConfig>(200, "success", results));
        }
    }
}
