using Microsoft.AspNetCore.Mvc;
using ServicePortal.Applications.Modules.UserConfig.Services.Interfaces;
using ServicePortal.Common;

namespace ServicePortal.Applications.Modules.UserConfig.Controllers
{
    [ApiController, Route("api/user-config")]
    public class UserConfigController : ControllerBase
    {
        private readonly IUserConfigService _userConfigService;
        public UserConfigController(IUserConfigService userConfigService)
        {
            _userConfigService = userConfigService;
        }

        [HttpPost("save-or-update")]
        public async Task<IActionResult> SaveOrUpdate(Domain.Entities.UserConfig request)
        {
            var results = await _userConfigService.SaveOrUpdate(request);

            var response = new BaseResponse<Domain.Entities.UserConfig>(200, "Success", results);

            return Ok(response);
        }

        [HttpGet("get-config-by-usercode-and-key")]
        public async Task<IActionResult> GetConfigByUserCodeAndkey(string userCode, string key)
        {
            var results = await _userConfigService.GetConfigByUserCodeAndkey(userCode, key);

            var response = new BaseResponse<Domain.Entities.UserConfig>(200, "Success", results);

            return Ok(response);
        }
    }
}
