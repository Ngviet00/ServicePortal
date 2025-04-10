using Microsoft.AspNetCore.Mvc;
using ServicePortal.Common;
using ServicePortal.Modules.User.DTO;
using ServicePortal.Modules.User.Interfaces;

namespace ServicePortal.Modules.User.Controllers
{
    //[Authorize]
    [ApiController, Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            List<UserDTO> users = await _userService.GetAll();

            return Ok(new BaseResponse<List<UserDTO>>(200, "Success", users));
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            UserDTO? userDTO = await _userService.GetById(id);

            return Ok(new BaseResponse<UserDTO>(200, "success", userDTO));
        }

        [HttpGet("get-by-code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            UserDTO? userDTO = await _userService.GetByCode(code);

            return Ok(new BaseResponse<UserDTO>(200, "success", userDTO));
        }

        //[HttpPut("/update")]
        //public async Task<IActionResult> Update(UserResponse user)
        //{
        //    UserResponse? userResponse = await _userService.Update(user);

        //    return Ok(new BaseResponse<UserResponse>(200, "Update user successfully", userResponse));
        //}

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.Delete(id);

            return Ok(new BaseResponse<UserDTO>(200, "Delete user successfully", null));
        }

        [HttpDelete("force-delete/{id}")]
        public async Task<IActionResult> ForceDelete(Guid id)
        {
            await _userService.ForceDelete(id);

            return Ok(new BaseResponse<UserDTO>(200, "Delete user permanently successfully", null));
        }
    }
}