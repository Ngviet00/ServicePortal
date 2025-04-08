namespace ServicePortal.Modules.User.Controllers
{
    public class UserController
    {
    }
}
//using Microsoft.AspNetCore.Mvc;
//using ServicePortal.Application.DTOs;
//using ServicePortal.Application.DTOs.User.Responses;
//using ServicePortal.Application.Services;

//namespace ServicePortal.Presentation.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class UserController : ControllerBase
//    {
//        private readonly UserService _userService;

//        public UserController(UserService userService)
//        {
//            _userService = userService;
//        }

//        [HttpGet("/get-all")]
//        public async Task<IActionResult> GetAll()
//        {
//            List<UserResponse> users = await _userService.GetAll();

//            return Ok(new BaseResponse<List<UserResponse>>(200, "Success", users));
//        }

//        [HttpGet("/{id}")]
//        public async Task<IActionResult> GetById(Guid id)
//        {
//            UserResponse? user = await _userService.GetById(id);

//            return Ok(new BaseResponse<UserResponse>(200, "success", user));
//        }

//        [HttpPut("/update")]
//        public async Task<IActionResult> Update(UserResponse user)
//        {
//            UserResponse? userResponse = await _userService.Update(user);

//            return Ok(new BaseResponse<UserResponse>(200, "Update user successfully", userResponse));
//        }

//        [HttpDelete("/delete/{id}")]
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            await _userService.Delete(id);

//            return Ok(new BaseResponse<UserResponse>(200, "Delete user successfully", null));
//        }

//        [HttpDelete("/force-delete/{id}")]
//        public async Task<IActionResult> ForceDelete(Guid id)
//        {
//            await _userService.ForceDelete(id);

//            return Ok(new BaseResponse<UserResponse>(200, "Delete user permanently successfully", null));
//        }
//    }
//}

////34f81e1e-eff0-443a-9704-2a4819961c52